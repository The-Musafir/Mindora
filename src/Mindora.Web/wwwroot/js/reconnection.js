// ============================================================
// Mindora — Global Reconnection Indicator
// Shows a subtle banner when SignalR is reconnecting.
// ============================================================
(function () {
    'use strict';

    let banner = null;
    let activeReconnecting = 0;

    document.addEventListener('DOMContentLoaded', () => {
        ensureBanner();
        hookNotificationHub();
    });

    function ensureBanner() {
        if (document.getElementById('reconnectBanner')) {
            banner = document.getElementById('reconnectBanner');
            return;
        }

        const div = document.createElement('div');
        div.id = 'reconnectBanner';
        div.className = 'reconnect-banner';
        div.innerHTML = `
            <span class="spinner-border spinner-border-sm me-2" role="status"></span>
            <span>Reconnecting...</span>
        `;
        document.body.appendChild(div);
        banner = div;
    }

    function showBanner() {
        activeReconnecting++;
        banner?.classList.add('show');
    }

    function hideBanner() {
        activeReconnecting = Math.max(0, activeReconnecting - 1);
        if (activeReconnecting === 0) {
            banner?.classList.remove('show');
        }
    }

    function hookNotificationHub() {
        let attempts = 0;
        const interval = setInterval(() => {
            attempts++;

            const conn = window.MindoraNotifications?.getConnection?.();
            if (conn) {
                attachHandlers(conn);
                clearInterval(interval);
                return;
            }

            if (attempts > 20) clearInterval(interval);
        }, 500);
    }

    function attachHandlers(conn) {
        if (!conn || conn.__reconnectHooked) return;
        conn.__reconnectHooked = true;

        conn.onreconnecting(() => {
            console.warn('[Reconnection] reconnecting...');
            showBanner();
        });

        conn.onreconnected((id) => {
            console.log('[Reconnection] reconnected:', id);
            hideBanner();
        });

        conn.onclose(() => {
            console.warn('[Reconnection] closed');
            hideBanner();
        });
    }
})();