using Mindora.Application.DTOs.Notification;

namespace Mindora.Application.Interfaces
{
  
    public interface IRealtimeNotificationService
    {
        
        Task SendToUserAsync(
            Guid userId,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default);

        
        Task SendToUsersAsync(
            IEnumerable<Guid> userIds,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default);

       
        Task SendToGroupAsync(
            string groupName,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default);

    
        Task BroadcastAsync(
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default);

   
        Task SendUnreadCountAsync(
            Guid userId,
            int unreadCount,
            CancellationToken cancellationToken = default);

  
        Task DismissForUserAsync(
            Guid userId,
            Guid notificationId,
            CancellationToken cancellationToken = default);
    }
}