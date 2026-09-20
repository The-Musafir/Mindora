using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Web.Services
{
    /// <summary>
    /// SignalR audit logger.
    /// Stores SignalR events as UserNotification entries (channel = "Audit", IsRead = true)
    /// so they don't show up in user's notification bell but remain queryable by admins.
    ///
    /// Rationale: Reuses existing NotificationLog infrastructure — no migration needed.
    /// </summary>
    public class SignalRAuditService : ISignalRAuditService
    {
        private readonly MindoraDbContext _context;
        private readonly ILogger<SignalRAuditService> _logger;

        // System user ID for audit entries (Guid.Empty = system)
        private static readonly Guid SystemUserId = Guid.Empty;

        public SignalRAuditService(
            MindoraDbContext context,
            ILogger<SignalRAuditService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(
            Guid userId,
            string eventType,
            string message,
            string? metadata = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Store as a UserNotification with:
                //   Channel = "Audit" → excluded from user bell
                //   Type = "SignalR:<eventType>" → filterable
                //   IsRead = true → never counts as unread
                var notification = new UserNotification
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = userId == Guid.Empty ? SystemUserId : userId,
                    Title = $"SignalR: {eventType}",
                    Body = message,
                    Channel = "Audit",
                    Type = $"SignalR:{eventType}",
                    IsRead = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserNotifications.Add(notification);

                _context.NotificationLogs.Add(new NotificationLog
                {
                    LogId = Guid.NewGuid(),
                    NotificationId = notification.NotificationId,
                    Status = "AuditLogged",
                    ErrorMessage = metadata,   // reuse column for extra context
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Audit failure must NEVER break SignalR
                _logger.LogWarning(ex,
                    "SignalRAuditService failed: {EventType} for user {UserId}",
                    eventType, userId);
            }
        }
    }
}