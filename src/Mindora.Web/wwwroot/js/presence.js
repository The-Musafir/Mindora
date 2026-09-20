// ============================================================
// Mindora — Presence Client
// Online/offline status via SignalR
// ============================================================
(function () {
    'use strict';

    const HUB_URL = '/hubs/presence';
    let connection = null;

    document.addEventListener('DOMContentLoaded', () => {
        startPresenceConnection();
    });

    async function startPresenceConnection() {
        if (!window.signalR) {
            console.warn('[Presence] signalR library not loaded');
            return;
        }

        connection = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL)
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        connection.on('UserOnline', userId => updatePresenceUI(userId, true));
        connection.on('UserOffline', userId => updatePresenceUI(userId, false));

        connection.onreconnected(id => console.log('[Presence] reconnected:', id));
        connection.onclose(() => console.warn('[Presence] disconnected'));

        try {
            await connection.start();
            console.log('[Presence] connected:', connection.connectionId);
        } catch (err) {
            console.error('[Presence] connection failed:', err);
            setTimeout(startPresenceConnection, 5000);
        }
    }

    /**
     * Update any DOM elements with [data-presence-user-id="<guid>"].
     * Adds class "online" or "offline" based on status.
     */
    function updatePresenceUI(userId, isOnline) {
        document.querySelectorAll(`[data-presence-user-id="${userId}"]`)
            .forEach(el => {
                el.classList.toggle('online', isOnline);
                el.classList.toggle('offline', !isOnline);
                el.setAttribute('title', isOnline ? 'Online' : 'Offline');
            });
    }

    // Public API for pages that need to query presence
    window.MindoraPresence = {
        isOnline: async (userId) => {
            try {
                const res = await fetch(
                    `/Presence/IsOnline?userId=${encodeURIComponent(userId)}`,
                    { credentials: 'include' });
                if (!res.ok) return false;
                const data = await res.json();
                return data.isOnline;
            } catch {
                return false;
            }
        },

        getOnlineUsers: async () => {
            try {
                const res = await fetch('/Presence/OnlineUsers', { credentials: 'include' });
                if (!res.ok) return { count: 0, userIds: [] };
                return await res.json();
            } catch {
                return { count: 0, userIds: [] };
            }
        },

        getConnectionId: () => connection?.connectionId ?? null
    };
})();