using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IConnectionManager _connectionManager;
        private readonly INotificationService _notificationService;
        private readonly ISignalRAuditService _audit;
        private readonly ILogger<NotificationHub> _logger;

        // Max concurrent connections per user (tabs/devices)
        private const int MAX_CONNECTIONS_PER_USER = 3;

        public NotificationHub(
            IConnectionManager connectionManager,
            INotificationService notificationService,
            ISignalRAuditService audit,
            ILogger<NotificationHub> logger)
        {
            _connectionManager = connectionManager;
            _notificationService = notificationService;
            _audit = audit;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                await base.OnConnectedAsync();
                return;
            }

            // ============================================================
            // CONNECTION LIMIT CHECK
            // ============================================================
            var existing = _connectionManager.GetConnections(userIdStr);

            if (existing.Count >= MAX_CONNECTIONS_PER_USER)
            {
                _logger.LogWarning(
                    "User {UserId} exceeded connection limit ({Count}/{Max}) — rejecting",
                    userIdStr, existing.Count, MAX_CONNECTIONS_PER_USER);

                await _audit.LogAsync(
                    Guid.TryParse(userIdStr, out var uid) ? uid : Guid.Empty,
                    "ConnectionLimitExceeded",
                    $"User had {existing.Count} active connections; limit is {MAX_CONNECTIONS_PER_USER}",
                    metadata: $"connId={Context.ConnectionId}");

                // Notify before aborting
                await Clients.Caller.SendAsync("ConnectionRejected", new
                {
                    reason = "Too many active connections",
                    limit = MAX_CONNECTIONS_PER_USER
                });

                Context.Abort();
                return;
            }

            // ============================================================
            // ACCEPT CONNECTION
            // ============================================================
            _connectionManager.AddConnection(userIdStr, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userIdStr}");

            _logger.LogInformation(
                "User {UserId} connected (Conn: {ConnId}, Total: {Total})",
                userIdStr, Context.ConnectionId, existing.Count + 1);

            // ============================================================
            // OFFLINE DELIVERY — push unread count on connect
            // ============================================================
            if (Guid.TryParse(userIdStr, out var userId))
            {
                try
                {
                    var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
                    await Clients.Caller.SendAsync("UnreadCountUpdated", unreadCount);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Unread count push failed for user {UserId}", userId);
                }
            }

            await Clients.Others.SendAsync("UserConnected", userIdStr);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userIdStr))
            {
                _connectionManager.RemoveConnection(userIdStr, Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userIdStr}");

                _logger.LogInformation(
                    "User {UserId} disconnected (Conn: {ConnId})",
                    userIdStr, Context.ConnectionId);

                if (!_connectionManager.IsOnline(userIdStr))
                {
                    await Clients.Others.SendAsync("UserDisconnected", userIdStr);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public Task<int> GetOnlineCount()
            => Task.FromResult(_connectionManager.GetOnlineUserCount());

        public async Task<int> RefreshUnreadCount()
        {
            var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return 0;

            return await _notificationService.GetUnreadCountAsync(userId);
        }
    }
}