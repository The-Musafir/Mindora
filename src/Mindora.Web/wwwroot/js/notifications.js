// ============================================================
// Mindora — Realtime Notifications Client
// SignalR connection + Toast + Bell Badge + Dropdown
// ============================================================
(function () {
    'use strict';

    const HUB_URL = '/hubs/notifications';
    const TOAST_DURATION = 5000;
    const RECONNECT_DELAYS = [0, 2000, 5000, 10000, 30000];

    let connection = null;
    let unreadCount = 0;
    let soundAudio = null;

    // ============================================================
    // INIT
    // ============================================================
    document.addEventListener('DOMContentLoaded', () => {
        ensureToastContainer();
        ensureBell();
        loadUnreadCountFromServer();
        startSignalRConnection();
    });

    // ============================================================
    // SIGNALR
    // ============================================================
    async function startSignalRConnection() {
        if (!window.signalR) {
            console.error('[Notifications] signalR library not loaded');
            return;
        }

        connection = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL)
            .withAutomaticReconnect(RECONNECT_DELAYS)
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        connection.on('ReceiveNotification', handleIncomingNotification);
        connection.on('UnreadCountUpdated', handleUnreadCountUpdate);
        connection.on('NotificationDismissed', removeFromDropdown);

        connection.onreconnecting(() => console.warn('[Notifications] reconnecting...'));
        connection.onreconnected(id => {
            console.log('[Notifications] reconnected:', id);
            loadUnreadCountFromServer();
        });
        connection.onclose(() => console.warn('[Notifications] closed'));

        try {
            await connection.start();
            console.log('[Notifications] connected:', connection.connectionId);
        } catch (err) {
            console.error('[Notifications] connection failed:', err);
            setTimeout(startSignalRConnection, 5000);
        }
    }

    function handleIncomingNotification(notif) {
        console.log('[Notifications] received:', notif);

        if (typeof notif.unreadCount === 'number') {
            unreadCount = notif.unreadCount;
        } else {
            unreadCount++;
        }
        updateBellBadge(unreadCount);

        showToast(notif);

        if (notif.isSoundEnabled) {
            playNotificationSound();
        }

        refreshDropdownIfOpen();
    }

    function handleUnreadCountUpdate(count) {
        unreadCount = count;
        updateBellBadge(count);
    }

    // ============================================================
    // BELL
    // ============================================================
    function ensureBell() {
        // ✅ If navbar already has a bell, just wire up handlers
        if (document.getElementById('notificationBell')) {
            document.getElementById('markAllReadBtn')
                ?.addEventListener('click', markAllRead);

            document.getElementById('notificationBell')
                ?.addEventListener('shown.bs.dropdown', loadDropdownContent);

            return;
        }

        // Fallback: no bell in navbar → auto-inject one
        const navContainer = findBellContainer();
        if (!navContainer) {
            console.warn('[Notifications] navbar container not found — bell not injected');
            return;
        }

        const bellHtml = `
            <li class="nav-item dropdown" id="notification-bell-container">
                <a class="nav-link position-relative" href="#" id="notificationBell"
                   role="button" data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="fas fa-bell"></i>
                    <span id="notificationBadge"
                          class="badge rounded-pill bg-danger position-absolute top-0 start-100 translate-middle d-none"
                          style="font-size: 0.65rem;">
                        0
                    </span>
                </a>
                <ul class="dropdown-menu dropdown-menu-end notification-dropdown"
                    aria-labelledby="notificationBell"
                    style="width: 380px; max-height: 500px; overflow-y: auto;">
                    <li class="dropdown-header d-flex justify-content-between align-items-center">
                        <strong>Notifications</strong>
                        <button id="markAllReadBtn" class="btn btn-sm btn-link p-0 text-decoration-none">Mark all read</button>
                    </li>
                    <li><hr class="dropdown-divider"></li>
                    <li id="notificationListContainer">
                        <div class="text-center py-3 text-muted small">
                            <span class="spinner-border spinner-border-sm"></span> Loading...
                        </div>
                    </li>
                    <li><hr class="dropdown-divider"></li>
                    <li>
                        <a class="dropdown-item text-center small" href="/Notification/Index">
                            View all notifications
                        </a>
                    </li>
                </ul>
            </li>
        `;

        navContainer.insertAdjacentHTML('beforeend', bellHtml);

        document.getElementById('markAllReadBtn')
            ?.addEventListener('click', markAllRead);

        document.getElementById('notificationBell')
            ?.addEventListener('shown.bs.dropdown', loadDropdownContent);

        updateBellBadge(0);
    }

    function findBellContainer() {
        return document.querySelector('.navbar-nav')
            || document.querySelector('#navbarNav')
            || document.querySelector('.navbar-collapse ul')
            || document.querySelector('nav .container ul')
            || null;
    }

    function updateBellBadge(count) {
        const badge = document.getElementById('notificationBadge');
        if (!badge) return;

        if (count > 0) {
            // Support both "badge" (auto-injected) and "dot" (navbar) styles
            if (badge.classList.contains('notification-dot')) {
                badge.classList.remove('d-none');
                badge.textContent = '';
            } else {
                badge.textContent = count > 99 ? '99+' : count;
                badge.classList.remove('d-none');
            }
        } else {
            badge.classList.add('d-none');
            if (!badge.classList.contains('notification-dot')) {
                badge.textContent = '0';
            }
        }
    }

    // ============================================================
    // DROPDOWN
    // ============================================================
    async function loadUnreadCountFromServer() {
        try {
            const res = await fetch('/Notification/GetUnreadCount', { credentials: 'include' });
            if (!res.ok) return;
            const data = await res.json();
            unreadCount = data.count ?? 0;
            updateBellBadge(unreadCount);
        } catch (err) {
            console.warn('[Notifications] unread count fetch failed:', err);
        }
    }

    async function loadDropdownContent() {
        const container = document.getElementById('notificationListContainer');
        if (!container) return;

        container.innerHTML = '<div class="text-center py-3 text-muted small"><span class="spinner-border spinner-border-sm"></span> Loading...</div>';

        try {
            const res = await fetch('/Notification/GetRecent', { credentials: 'include' });
            if (!res.ok) throw new Error('HTTP ' + res.status);
            const items = await res.json();

            if (!items || items.length === 0) {
                container.innerHTML = '<div class="text-center py-3 text-muted small">No notifications yet</div>';
                return;
            }

            container.innerHTML = items.map(renderNotificationItem).join('');
            attachDropdownHandlers();
        } catch (err) {
            console.error('[Notifications] load failed:', err);
            container.innerHTML = '<div class="text-center py-3 text-danger small">Failed to load</div>';
        }
    }

    function renderNotificationItem(n) {
        const isUnread = !n.isRead;
        const bg = isUnread ? 'bg-light' : '';
        const bold = isUnread ? 'fw-semibold' : '';

        return `
            <div class="dropdown-item notification-item ${bg}" data-id="${n.notificationId}">
                <div class="d-flex">
                    <div class="me-2 text-primary"><i class="fas ${n.icon || 'fa-bell'}"></i></div>
                    <div class="flex-grow-1">
                        <div class="${bold} small">${escapeHtml(n.title)}</div>
                        <div class="text-muted small">${escapeHtml(n.body)}</div>
                        <div class="text-muted" style="font-size:0.7rem;">${formatTime(n.createdAt)}</div>
                    </div>
                </div>
            </div>
        `;
    }

    function attachDropdownHandlers() {
        document.querySelectorAll('.notification-item').forEach(el => {
            el.addEventListener('click', async () => {
                const id = el.dataset.id;
                if (!id) return;

                el.classList.remove('bg-light');
                const titleEl = el.querySelector('.fw-semibold');
                if (titleEl) titleEl.classList.remove('fw-semibold');

                if (unreadCount > 0) {
                    unreadCount--;
                    updateBellBadge(unreadCount);
                }
                await markOneRead(id);
            });
        });
    }

    function refreshDropdownIfOpen() {
        const menu = document.querySelector('.notification-dropdown.show');
        if (menu) loadDropdownContent();
    }

    function removeFromDropdown(notificationId) {
        const el = document.querySelector(`.notification-item[data-id="${notificationId}"]`);
        if (el) el.remove();
    }

    // ============================================================
    // ACTIONS
    // ============================================================
    async function markOneRead(notificationId) {
        try {
            await fetch('/Notification/MarkAsReadAjax', {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id: notificationId })
            });
        } catch (err) {
            console.warn('[Notifications] mark read failed:', err);
        }
    }

    async function markAllRead() {
        try {
            await fetch('/Notification/MarkAllAsReadAjax', {
                method: 'POST',
                credentials: 'include'
            });
            unreadCount = 0;
            updateBellBadge(0);
            loadDropdownContent();
        } catch (err) {
            console.warn('[Notifications] mark all failed:', err);
        }
    }

    // ============================================================
    // TOAST
    // ============================================================
    function ensureToastContainer() {
        if (document.getElementById('notificationToastContainer')) return;

        const div = document.createElement('div');
        div.id = 'notificationToastContainer';
        div.className = 'notification-toast-container';
        document.body.appendChild(div);
    }

    function showToast(notif) {
        const container = document.getElementById('notificationToastContainer');
        if (!container) return;

        const toast = document.createElement('div');
        toast.className = `notification-toast severity-${(notif.severity || 'info').toLowerCase()}`;
        toast.innerHTML = `
            <div class="d-flex align-items-start">
                <div class="me-2 fs-4"><i class="fas ${notif.icon || 'fa-bell'}"></i></div>
                <div class="flex-grow-1">
                    <div class="fw-bold">${escapeHtml(notif.title || '')}</div>
                    <div class="small">${escapeHtml(notif.body || '')}</div>
                </div>
                <button type="button" class="btn-close ms-2" aria-label="Close"></button>
            </div>
        `;

        toast.querySelector('.btn-close')?.addEventListener('click', () => removeToast(toast));
        container.appendChild(toast);

        requestAnimationFrame(() => toast.classList.add('notification-toast-show'));
        setTimeout(() => removeToast(toast), TOAST_DURATION);
    }

    function removeToast(toast) {
        if (!toast || !toast.parentElement) return;
        toast.classList.remove('notification-toast-show');
        setTimeout(() => toast.remove(), 300);
    }

    // ============================================================
    // SOUND
    // ============================================================
    function playNotificationSound() {
        try {
            if (!soundAudio) {
                soundAudio = new Audio('/sounds/notification.mp3');
                soundAudio.volume = 0.4;
            }
            soundAudio.play().catch(() => { /* autoplay blocked */ });
        } catch (err) { /* ignore */ }
    }

    // ============================================================
    // HELPERS
    // ============================================================
    function escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function formatTime(iso) {
        if (!iso) return '';
        const d = new Date(iso);
        const diff = (Date.now() - d.getTime()) / 1000;
        if (diff < 60) return 'just now';
        if (diff < 3600) return Math.floor(diff / 60) + 'm ago';
        if (diff < 86400) return Math.floor(diff / 3600) + 'h ago';
        return d.toLocaleDateString();
    }

    // expose for debugging
    window.MindoraNotifications = {
        getConnection: () => connection,
        getUnreadCount: () => unreadCount,
        showToast
    };
})();