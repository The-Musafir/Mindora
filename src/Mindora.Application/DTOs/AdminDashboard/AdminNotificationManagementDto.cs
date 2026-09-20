using Mindora.Application.DTOs.Notification;

namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminNotificationManagementDto
    {
        public List<NotificationTemplateDto> Templates { get; set; } = new();
        public List<NotificationLogDto> RecentLogs { get; set; } = new();
    }
}