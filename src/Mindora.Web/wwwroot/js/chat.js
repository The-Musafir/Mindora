// ============================================================
// Mindora — AI Chat Client (SignalR streaming + Sidebar + Menu)
// ============================================================
(function () {
    'use strict';

    const sessionId = document.getElementById('sessionId')?.value;
    if (!sessionId) return;

    let connection = null;
    let isStreaming = false;

    document.addEventListener('DOMContentLoaded', async () => {
        bindEvents();
        scrollToBottom();
        await startChatConnection();
    });

    // ============================================================
    // SIGNALR
    // ============================================================
    async function startChatConnection() {
        if (!window.signalR) {
            console.error('[Chat] signalR not loaded');
            return;
        }

        connection = new signalR.HubConnectionBuilder()
            .withUrl('/hubs/chat')
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        connection.on('AIStreamStarted', onStreamStarted);
        connection.on('AIStreamChunk', onStreamChunk);
        connection.on('AIStreamCompleted', onStreamCompleted);
        connection.on('AIStreamError', onStreamError);

        try {
            await connection.start();
            console.log('[Chat] connected:', connection.connectionId);
        } catch (err) {
            console.error('[Chat] connect failed:', err);
        }
    }

    // ============================================================
    // UI BINDING
    // ============================================================
    function bindEvents() {
        document.getElementById('chatForm')
            ?.addEventListener('submit', onSubmit);

        document.getElementById('stopBtn')
            ?.addEventListener('click', onStop);

        document.getElementById('toggleSidebar')
            ?.addEventListener('click', toggleSidebar);

        bindSessionContextMenu();
    }

    function toggleSidebar() {
        document.getElementById('chatSidebar')?.classList.toggle('open');
    }

    function onSubmit(e) {
        e.preventDefault();
        if (isStreaming) return;

        const input = document.getElementById('messageInput');
        const text = input.value.trim();
        if (!text) return;

        document.getElementById('emptyState')?.remove();

        appendMessage(text, 'user');
        scrollToBottom();

        connection.invoke('StartAIChat', sessionId, text)
            .catch(err => console.error('[Chat] invoke failed:', err));

        input.value = '';
        input.focus();
    }

    function onStop() {
        isStreaming = false;
        hideStreamingIndicator();
        setSendMode();
    }

    // ============================================================
    // STREAMING EVENTS
    // ============================================================
    function onStreamStarted(payload) {
        isStreaming = true;
        document.getElementById('streamingText').textContent = '';
        document.getElementById('streamingIndicator').classList.remove('d-none');
        setStopMode();
        scrollToBottom();
    }

    function onStreamChunk(payload) {
        const target = document.getElementById('streamingText');
        if (target) target.textContent = payload.accumulated;
        scrollToBottom();
    }

    function onStreamCompleted(payload) {
        isStreaming = false;
        hideStreamingIndicator();
        appendMessage(payload.fullContent, 'ai');
        setSendMode();
        scrollToBottom();
    }

    function onStreamError(payload) {
        isStreaming = false;
        hideStreamingIndicator();
        setSendMode();

        if (payload.error !== 'cancelled') {
            appendMessage('⚠️ ' + (payload.error || 'Something went wrong'), 'ai');
        }
        scrollToBottom();
    }

    // ============================================================
    // DOM HELPERS
    // ============================================================
    function appendMessage(text, sender) {
        const container = document.getElementById('chatMessages');
        const div = document.createElement('div');
        div.className = `chat-message chat-${sender}`;

        const time = new Date().toLocaleTimeString([], {
            hour: '2-digit',
            minute: '2-digit'
        });

        div.innerHTML = `
            <div class="chat-bubble">
                <div class="chat-text"></div>
                <div class="chat-time">${time}</div>
            </div>
        `;
        div.querySelector('.chat-text').textContent = text;
        container.appendChild(div);
    }

    function hideStreamingIndicator() {
        document.getElementById('streamingIndicator')?.classList.add('d-none');
    }

    function setSendMode() {
        document.getElementById('sendBtn')?.classList.remove('d-none');
        document.getElementById('stopBtn')?.classList.add('d-none');
        document.getElementById('messageInput').disabled = false;
    }

    function setStopMode() {
        document.getElementById('sendBtn')?.classList.add('d-none');
        document.getElementById('stopBtn')?.classList.remove('d-none');
        document.getElementById('messageInput').disabled = true;
    }

    function scrollToBottom() {
        const container = document.getElementById('chatContainer');
        if (container) {
            requestAnimationFrame(() => {
                container.scrollTop = container.scrollHeight;
            });
        }
    }

    // ============================================================
    // SESSION CONTEXT MENU (Rename / Delete)
    // ============================================================
    let activeMenuSessionId = null;

    function bindSessionContextMenu() {
        const contextMenu = document.getElementById('sessionContextMenu');
        if (!contextMenu) return;

        document.addEventListener('click', (e) => {
            const menuBtn = e.target.closest('.chat-session-menu-btn');
            if (menuBtn) {
                e.preventDefault();
                e.stopPropagation();
                openContextMenu(menuBtn);
                return;
            }

            if (!contextMenu.contains(e.target)) {
                hideContextMenu();
            }
        });

        contextMenu.querySelectorAll('.session-context-item').forEach(item => {
            item.addEventListener('click', async (e) => {
                e.preventDefault();
                const action = item.dataset.action;
                const targetId = activeMenuSessionId;
                const sessionEl = document.querySelector(`.chat-session-item[data-session-id="${targetId}"]`);
                const currentTitle = sessionEl?.dataset.sessionTitle || '';
                hideContextMenu();

                if (!targetId) return;

                if (action === 'delete') {
                    if (!confirm('Delete this chat? All messages will be permanently removed.')) return;
                    await deleteSession(targetId);
                } else if (action === 'rename') {
                    const newTitle = prompt('Enter new title:', currentTitle);
                    if (newTitle === null) return;
                    await renameSession(targetId, newTitle);
                }
            });
        });
    }

    function openContextMenu(btn) {
        const contextMenu = document.getElementById('sessionContextMenu');
        if (!contextMenu) return;

        activeMenuSessionId = btn.dataset.sessionId;

        const rect = btn.getBoundingClientRect();
        const menuWidth = 160;

        let left = rect.right - menuWidth;
        if (left < 8) left = 8;

        contextMenu.style.top = `${rect.bottom + 4}px`;
        contextMenu.style.left = `${left}px`;
        contextMenu.classList.remove('d-none');
    }

    function hideContextMenu() {
        document.getElementById('sessionContextMenu')?.classList.add('d-none');
        activeMenuSessionId = null;
    }

    async function deleteSession(targetId) {
        try {
            const res = await fetch('/Chat/DeleteSession', {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ sessionId: targetId })
            });

            if (!res.ok) {
                alert('Failed to delete. Please try again.');
                return;
            }

            const el = document.querySelector(`.chat-session-item[data-session-id="${targetId}"]`);
            el?.remove();

            const currentSessionId = document.getElementById('sessionId')?.value;
            if (currentSessionId === targetId) {
                window.location.href = '/Chat';
                return;
            }

            if (document.querySelectorAll('.chat-session-item').length === 0) {
                window.location.reload();
            }
        } catch (err) {
            console.error('[Chat] delete failed:', err);
            alert('Something went wrong.');
        }
    }

    async function renameSession(targetId, newTitle) {
        try {
            const res = await fetch('/Chat/RenameSession', {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ sessionId: targetId, title: newTitle })
            });

            if (!res.ok) {
                alert('Failed to rename.');
                return;
            }

            const el = document.querySelector(`.chat-session-item[data-session-id="${targetId}"]`);
            if (el) {
                const titleEl = el.querySelector('.chat-session-title');
                const displayTitle = (newTitle || '').trim() || 'New Chat';
                if (titleEl) titleEl.textContent = displayTitle;
                el.dataset.sessionTitle = displayTitle;
            }
        } catch (err) {
            console.error('[Chat] rename failed:', err);
            alert('Something went wrong.');
        }
    }
})();