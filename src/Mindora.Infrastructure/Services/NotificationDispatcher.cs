using Microsoft.Extensions.Logging;
using Mindora.Application.DTOs.Notification;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly INotificationService _notificationService;
        private readonly IRealtimeNotificationService _realtimeNotifier;
        private readonly MindoraDbContext _context;
        private readonly ILogger<NotificationDispatcher> _logger;

        public NotificationDispatcher(
            INotificationService notificationService,
            IRealtimeNotificationService realtimeNotifier,
            MindoraDbContext context,
            ILogger<NotificationDispatcher> logger)
        {
            _notificationService = notificationService;
            _realtimeNotifier = realtimeNotifier;
            _context = context;
            _logger = logger;
        }

        // ================================================================
        // DispatchAsync — single user
        // ================================================================
        public async Task DispatchAsync(
            Guid userId,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty || notification == null)
            {
                _logger.LogWarning("DispatchAsync called with invalid parameters");
                return;
            }

            // ১. Preference check
            if (!await IsInAppEnabledAsync(userId, cancellationToken))
            {
                _logger.LogDebug("User {UserId} has InApp notifications disabled", userId);
                return;
            }

            if (await IsInQuietHoursAsync(userId, cancellationToken))
            {
                _logger.LogDebug("User {UserId} is in quiet hours", userId);
                return;
            }

            // ২. DB তে save
            var notificationId = await SaveToDatabaseAsync(userId, notification, cancellationToken);
            notification.NotificationId = notificationId;
            notification.UserId = userId;

            // ৩. Unread count বের করা
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
            notification.UnreadCount = unreadCount;

            // ৪. SignalR push (offline হলে skip হবে ভিতরে)
            await _realtimeNotifier.SendToUserAsync(userId, notification, cancellationToken);

            _logger.LogInformation(
                "📨 Dispatched notification {NotificationId} to user {UserId} | Type: {Type} | Unread: {Count}",
                notificationId, userId, notification.Type, unreadCount);
        }

        // ================================================================
        // DispatchToManyAsync — multiple users
        // ================================================================
        public async Task DispatchToManyAsync(
            IEnumerable<Guid> userIds,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default)
        {
            if (userIds == null || notification == null) return;

            var targets = userIds.Where(id => id != Guid.Empty).Distinct().ToList();
            if (targets.Count == 0) return;

            _logger.LogInformation(
                "📢 Dispatching notification to {Count} users | Type: {Type}",
                targets.Count, notification.Type);

            foreach (var userId in targets)
            {
                if (cancellationToken.IsCancellationRequested) break;

                try
                {
                    // প্রতি user এর জন্য আলাদা DTO (userId override)
                    var userDto = CloneFor(notification, userId);
                    await DispatchAsync(userId, userDto, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to dispatch notification to user {UserId}", userId);
                }
            }
        }

        // ================================================================
        // SaveOnlyAsync — DB only (no SignalR)
        // ================================================================
        public async Task<Guid> SaveOnlyAsync(
            Guid userId,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty || notification == null)
                return Guid.Empty;

            var notificationId = await SaveToDatabaseAsync(userId, notification, cancellationToken);

            _logger.LogInformation(
                "💾 Saved notification {NotificationId} to DB for user {UserId} (no push)",
                notificationId, userId);

            return notificationId;
        }

        // ================================================================
        // PRIVATE HELPERS
        // ================================================================

        private async Task<Guid> SaveToDatabaseAsync(
            Guid userId,
            RealtimeNotificationDto dto,
            CancellationToken cancellationToken)
        {
            var entity = new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = userId,
                TemplateId = dto.TemplateId,
                Title = dto.Title,
                Body = dto.Body,
                Channel = dto.Channel ?? "InApp",
                Type = dto.Type ?? "General",
                ReferenceId = dto.ReferenceId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserNotifications.Add(entity);

            _context.NotificationLogs.Add(new NotificationLog
            {
                LogId = Guid.NewGuid(),
                NotificationId = entity.NotificationId,
                Status = "Sent",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(cancellationToken);

            return entity.NotificationId;
        }

        private async Task<bool> IsInAppEnabledAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var preference = await _context.UserNotificationPreferences
                .FindAsync(new object[] { userId }, cancellationToken);

            // Preference না থাকলে default = enabled
            return preference?.InAppEnabled ?? true;
        }

        private async Task<bool> IsInQuietHoursAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var preference = await _context.UserNotificationPreferences
                .FindAsync(new object[] { userId }, cancellationToken);

            if (preference == null || !preference.QuietHoursEnabled)
                return false;

            if (!preference.QuietHoursStart.HasValue || !preference.QuietHoursEnd.HasValue)
                return false;

            var now = DateTime.UtcNow.TimeOfDay;
            var start = preference.QuietHoursStart.Value;
            var end = preference.QuietHoursEnd.Value;

            // Same day: 09:00 - 17:00
            if (start <= end)
                return now >= start && now <= end;

            // Overnight: 22:00 - 07:00 (spans midnight)
            return now >= start || now <= end;
        }

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
    }
}