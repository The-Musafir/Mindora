using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Notification;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly MindoraDbContext _context;

        public NotificationService(MindoraDbContext context)
        {
            _context = context;
        }

        // ============================
        // USER NOTIFICATIONS
        // ============================

        public async Task<IReadOnlyList<NotificationDto>> GetUserNotificationsAsync(Guid userId)
        {
            var notifications = await _context.UserNotifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(MapNotificationToDto).ToList();
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(Guid notificationId)
        {
            var notification = await _context.UserNotifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

            return notification == null ? null : MapNotificationToDto(notification);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.UserNotifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<Guid> CreateNotificationAsync(CreateNotificationRequest request)
        {
            var notification = new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = request.UserId,
                Title = request.Title,
                Body = request.Body,
                Channel = request.Channel,
                Type = request.Type,
                ReferenceId = request.ReferenceId,
                TemplateId = request.TemplateId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserNotifications.Add(notification);

            // log entry (Sent)
            _context.NotificationLogs.Add(new NotificationLog
            {
                LogId = Guid.NewGuid(),
                NotificationId = notification.NotificationId,
                Status = "Sent",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return notification.NotificationId;
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var notification = await _context.UserNotifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification == null) return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            // update log
            var log = await _context.NotificationLogs
                .FirstOrDefaultAsync(l => l.NotificationId == notificationId);
            if (log != null)
                log.Status = "Read";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _context.UserNotifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (!unreadNotifications.Any()) return false;

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;

                var log = await _context.NotificationLogs
                    .FirstOrDefaultAsync(l => l.NotificationId == notification.NotificationId);
                if (log != null)
                    log.Status = "Read";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId)
        {
            var notification = await _context.UserNotifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification == null) return false;

            _context.UserNotifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // NOTIFICATION PREFERENCES
        // ============================

        public async Task<NotificationPreferenceDto?> GetUserPreferenceAsync(Guid userId)
        {
            var preference = await _context.UserNotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return preference == null ? null : MapPreferenceToDto(preference);
        }

        public async Task<NotificationPreferenceDto> UpdatePreferenceAsync(Guid userId, UpdateNotificationPreferenceRequest request)
        {
            var preference = await _context.UserNotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (preference == null)
            {
                preference = new UserNotificationPreference
                {
                    PreferenceId = Guid.NewGuid(),
                    UserId = userId,
                    EmailEnabled = request.EmailEnabled,
                    PushEnabled = request.PushEnabled,
                    InAppEnabled = request.InAppEnabled,
                    QuietHoursEnabled = request.QuietHoursEnabled,
                    QuietHoursStart = request.QuietHoursStart,
                    QuietHoursEnd = request.QuietHoursEnd,
                    Frequency = request.Frequency
                };
                _context.UserNotificationPreferences.Add(preference);
            }
            else
            {
                preference.EmailEnabled = request.EmailEnabled;
                preference.PushEnabled = request.PushEnabled;
                preference.InAppEnabled = request.InAppEnabled;
                preference.QuietHoursEnabled = request.QuietHoursEnabled;
                preference.QuietHoursStart = request.QuietHoursStart;
                preference.QuietHoursEnd = request.QuietHoursEnd;
                preference.Frequency = request.Frequency;
            }

            await _context.SaveChangesAsync();
            return MapPreferenceToDto(preference);
        }

        // ============================
        // NOTIFICATION TEMPLATES
        // ============================

        public async Task<IReadOnlyList<NotificationTemplateDto>> GetAllTemplatesAsync()
        {
            var templates = await _context.NotificationTemplates.ToListAsync();
            return templates.Select(t => new NotificationTemplateDto
            {
                TemplateId = t.TemplateId,
                TemplateKey = t.TemplateKey,
                Subject = t.Subject,
                BodyTemplate = t.BodyTemplate,
                Channel = t.Channel,
                IsActive = t.IsActive
            }).ToList();
        }

        public async Task<Guid> CreateTemplateAsync(NotificationTemplateDto dto)
        {
            var template = new NotificationTemplate
            {
                TemplateId = Guid.NewGuid(),
                TemplateKey = dto.TemplateKey,
                Subject = dto.Subject,
                BodyTemplate = dto.BodyTemplate,
                Channel = dto.Channel,
                IsActive = dto.IsActive
            };

            _context.NotificationTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template.TemplateId;
        }

        public async Task<bool> ToggleTemplateActiveAsync(Guid templateId, bool isActive)
        {
            var template = await _context.NotificationTemplates.FindAsync(templateId);
            if (template == null) return false;

            template.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // NOTIFICATION LOGS (AUDIT)
        // ============================

        public async Task<IReadOnlyList<NotificationLogDto>> GetNotificationLogsAsync(Guid notificationId)
        {
            var logs = await _context.NotificationLogs
                .Where(l => l.NotificationId == notificationId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logs.Select(l => new NotificationLogDto
            {
                LogId = l.LogId,
                NotificationId = l.NotificationId,
                Status = l.Status,
                ErrorMessage = l.ErrorMessage,
                CreatedAt = l.CreatedAt
            }).ToList();
        }

        // ============================
        // PRIVATE MAPPING HELPERS
        // ============================

        private NotificationDto MapNotificationToDto(UserNotification notification)
        {
            return new NotificationDto
            {
                NotificationId = notification.NotificationId,
                Title = notification.Title,
                Body = notification.Body,
                Channel = notification.Channel,
                Type = notification.Type,
                ReferenceId = notification.ReferenceId,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt
            };
        }

        private NotificationPreferenceDto MapPreferenceToDto(UserNotificationPreference preference)
        {
            return new NotificationPreferenceDto
            {
                PreferenceId = preference.PreferenceId,
                EmailEnabled = preference.EmailEnabled,
                PushEnabled = preference.PushEnabled,
                InAppEnabled = preference.InAppEnabled,
                QuietHoursEnabled = preference.QuietHoursEnabled,
                QuietHoursStart = preference.QuietHoursStart,
                QuietHoursEnd = preference.QuietHoursEnd,
                Frequency = preference.Frequency
            };
        }
    }
}