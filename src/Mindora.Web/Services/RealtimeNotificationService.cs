using Microsoft.AspNetCore.SignalR;
using Mindora.Application.DTOs.Notification;
using Mindora.Application.Interfaces;
using Mindora.Web.Hubs;
using Mindora.Web.Realtime;

namespace Mindora.Web.Services
{
    /// <summary>
    /// IRealtimeNotificationService এর SignalR-ভিত্তিক implementation।
    /// Hub এর বাইরে থেকেও notification push করতে পারে (IHubContext এর মাধ্যমে)।
    /// </summary>
    public class RealtimeNotificationService : IRealtimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<RealtimeNotificationService> _logger;

        public RealtimeNotificationService(
            IHubContext<NotificationHub> hubContext,
            IConnectionManager connectionManager,
            ILogger<RealtimeNotificationService> logger)
        {
            _hubContext = hubContext;
            _connectionManager = connectionManager;
            _logger = logger;
        }

        // ================================================================
        // PRIVATE HELPERS
        // ================================================================

        private static string GetUserGroupName(Guid userId) => $"user-{userId}";

        private static RealtimeNotificationDto CloneFor(
            RealtimeNotificationDto source,
            Guid userId)
        {
            return new RealtimeNotificationDto
            {
                NotificationId = source.NotificationId,
                UserId = userId,
                TemplateId = source.TemplateId,
                Title = source.Title,
                Body = source.Body,
                Channel = source.Channel,
                Type = source.Type,
                ReferenceId = source.ReferenceId,
                IsRead = source.IsRead,
                CreatedAt = source.CreatedAt,
                ReadAt = source.ReadAt,
                Severity = source.Severity,
                ActionUrl = source.ActionUrl,
                Icon = source.Icon,
                UnreadCount = source.UnreadCount,
                IsSoundEnabled = source.IsSoundEnabled
            };
        }

        // ================================================================
        // PUBLIC METHODS
        // ================================================================

        public async Task SendToUserAsync(
            Guid userId,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("SendToUserAsync called with empty userId");
                return;
            }

            if (notification == null)
            {
                _logger.LogWarning("SendToUserAsync called with null notification");
                return;
            }

            var userIdStr = userId.ToString();
            if (!_connectionManager.IsOnline(userIdStr))
            {
                _logger.LogDebug(
                    "User {UserId} is offline — skipping realtime push for notification {NotificationId}",
                    userId, notification.NotificationId);
                return;
            }

            notification.UserId = userId;

            try
            {
                var groupName = GetUserGroupName(userId);

                await _hubContext.Clients
                    .Group(groupName)
                    .SendAsync(HubEvents.ReceiveNotification, notification, cancellationToken);

                _logger.LogInformation(
                    "📨 Realtime notification pushed → User {UserId} | Type: {Type} | Severity: {Severity}",
                    userId, notification.Type, notification.Severity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to push realtime notification {NotificationId} to user {UserId}",
                    notification.NotificationId, userId);
            }
        }

        public async Task SendToUsersAsync(
            IEnumerable<Guid> userIds,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default)
        {
            if (userIds == null || notification == null) return;

            var targets = userIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (targets.Count == 0) return;

            var tasks = targets.Select(userId =>
                SendToUserAsync(userId, CloneFor(notification, userId), cancellationToken));

            await Task.WhenAll(tasks);

            _logger.LogInformation(
                "📨 Realtime notification pushed to {Count} users | Type: {Type}",
                targets.Count, notification.Type);
        }

        public async Task SendToGroupAsync(
            string groupName,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                _logger.LogWarning("SendToGroupAsync called with empty groupName");
                return;
            }

            if (notification == null) return;

            try
            {
                await _hubContext.Clients
                    .Group(groupName)
                    .SendAsync(HubEvents.ReceiveNotification, notification, cancellationToken);

                _logger.LogInformation(
                    "📨 Realtime notification pushed → Group {Group} | Type: {Type}",
                    groupName, notification.Type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to push to group {Group}", groupName);
            }
        }

        public async Task BroadcastAsync(
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default)
        {
            if (notification == null) return;

            try
            {
                await _hubContext.Clients.All
                    .SendAsync(HubEvents.ReceiveNotification, notification, cancellationToken);

                _logger.LogInformation(
                    "📢 Broadcast notification sent | Type: {Type} | Severity: {Severity}",
                    notification.Type, notification.Severity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Broadcast failed");
            }
        }

        public async Task SendUnreadCountAsync(
            Guid userId,
            int unreadCount,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty) return;

            try
            {
                var groupName = GetUserGroupName(userId);

                await _hubContext.Clients
                    .Group(groupName)
                    .SendAsync(HubEvents.UnreadCountUpdated, unreadCount, cancellationToken);

                _logger.LogDebug(
                    "🔢 Unread count synced → User {UserId} | Count: {Count}",
                    userId, unreadCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync unread count for {UserId}", userId);
            }
        }

        public async Task DismissForUserAsync(
            Guid userId,
            Guid notificationId,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty || notificationId == Guid.Empty) return;

            try
            {
                var groupName = GetUserGroupName(userId);

                await _hubContext.Clients
                    .Group(groupName)
                    .SendAsync(HubEvents.NotificationDismissed, notificationId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to dismiss notification {NotificationId} for {UserId}",
                    notificationId, userId);
            }
        }
    }
}