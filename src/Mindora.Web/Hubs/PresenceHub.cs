using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Hubs
{
    /// <summary>
    /// Presence Hub — tracks online/offline status.
    /// Uses shared IConnectionManager (same instance as NotificationHub).
    /// </summary>
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<PresenceHub> _logger;

        public PresenceHub(
            IConnectionManager connectionManager,
            ILogger<PresenceHub> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                var wasOffline = !_connectionManager.IsOnline(userId);

                _connectionManager.AddConnection(userId, Context.ConnectionId);

                await Groups.AddToGroupAsync(Context.ConnectionId, $"presence-{userId}");

                if (wasOffline)
                {
                    // First connection → broadcast online status
                    await Clients.Others.SendAsync("UserOnline", userId);
                    _logger.LogInformation("User {UserId} came online", userId);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                _connectionManager.RemoveConnection(userId, Context.ConnectionId);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"presence-{userId}");

                if (!_connectionManager.IsOnline(userId))
                {
                    // Last connection closed → broadcast offline status
                    await Clients.Others.SendAsync("UserOffline", userId);
                    _logger.LogInformation("User {UserId} went offline", userId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Client-callable methods
        public Task<bool> IsUserOnline(string userId)
            => Task.FromResult(_connectionManager.IsOnline(userId));

        public Task<IReadOnlyList<string>> GetOnlineUsers()
            => Task.FromResult(_connectionManager.GetOnlineUserIds());

        public Task<int> GetOnlineCount()
            => Task.FromResult(_connectionManager.GetOnlineUserCount());
    }
}