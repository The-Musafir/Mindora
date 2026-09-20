using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.DTOs.Notification;

namespace Mindora.Application.Interfaces
{
    public interface IAdminNotificationManagementService
    {
        Task<AdminNotificationManagementDto> GetManagementDataAsync();
        Task<IReadOnlyList<NotificationTemplateDto>> GetAllTemplatesAsync();
        Task<IReadOnlyList<NotificationLogDto>> GetRecentLogsAsync();
        Task<Guid> CreateTemplateAsync(NotificationTemplateDto dto);
        Task<bool> ToggleTemplateAsync(Guid templateId, bool isActive);
        Task<int> BroadcastAsync(BroadcastNotificationRequest request);
    }
}