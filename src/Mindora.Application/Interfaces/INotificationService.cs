using Mindora.Application.DTOs.Notification;

namespace Mindora.Application.Interfaces
{
    public interface INotificationService
    {
        // User Notifications
        Task<IReadOnlyList<NotificationDto>> GetUserNotificationsAsync(Guid userId);
        Task<NotificationDto?> GetNotificationByIdAsync(Guid notificationId);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<Guid> CreateNotificationAsync(CreateNotificationRequest request);
        Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId);
        Task<bool> MarkAllAsReadAsync(Guid userId);
        Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId);

        // Notification Preferences
        Task<NotificationPreferenceDto?> GetUserPreferenceAsync(Guid userId);
        Task<NotificationPreferenceDto> UpdatePreferenceAsync(Guid userId, UpdateNotificationPreferenceRequest request);

        // Notification Templates
        Task<IReadOnlyList<NotificationTemplateDto>> GetAllTemplatesAsync();
        Task<Guid> CreateTemplateAsync(NotificationTemplateDto dto);
        Task<bool> ToggleTemplateActiveAsync(Guid templateId, bool isActive);

        // Notification Logs (for admin/audit)
        Task<IReadOnlyList<NotificationLogDto>> GetNotificationLogsAsync(Guid notificationId);
    }
}