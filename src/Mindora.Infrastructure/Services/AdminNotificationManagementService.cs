using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.DTOs.Notification;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminNotificationManagementService : IAdminNotificationManagementService
    {
        private readonly MindoraDbContext _context;

        public AdminNotificationManagementService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<AdminNotificationManagementDto> GetManagementDataAsync()
        {
            return new AdminNotificationManagementDto
            {
                Templates = (await GetAllTemplatesAsync()).ToList(),
                RecentLogs = (await GetRecentLogsAsync()).ToList()
            };
        }

        public async Task<IReadOnlyList<NotificationTemplateDto>> GetAllTemplatesAsync()
        {
            var templates = await _context.NotificationTemplates
                .OrderByDescending(t => t.TemplateId)
                .ToListAsync();

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

        public async Task<IReadOnlyList<NotificationLogDto>> GetRecentLogsAsync()
        {
            var logs = await _context.NotificationLogs
                .OrderByDescending(l => l.CreatedAt)
                .Take(20)
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

        public async Task<bool> ToggleTemplateAsync(Guid templateId, bool isActive)
        {
            var template = await _context.NotificationTemplates.FindAsync(templateId);
            if (template == null) return false;

            template.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> BroadcastAsync(BroadcastNotificationRequest request)
        {
            var userIds = await _context.Users
                .Where(u => u.IsActive && !u.IsDeleted)
                .Select(u => u.Id)
                .ToListAsync();

            int count = 0;
            foreach (var userId in userIds)
            {
                var notification = new UserNotification
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = userId,
                    Title = request.Title,
                    Body = request.Body,
                    Channel = request.Channel,
                    Type = "Broadcast",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.UserNotifications.Add(notification);

                _context.NotificationLogs.Add(new NotificationLog
                {
                    LogId = Guid.NewGuid(),
                    NotificationId = notification.NotificationId,
                    Status = "Sent",
                    CreatedAt = DateTime.UtcNow
                });

                count++;
            }

            await _context.SaveChangesAsync();
            return count;
        }
    }
}