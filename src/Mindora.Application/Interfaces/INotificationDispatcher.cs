using Mindora.Application.DTOs.Notification;

namespace Mindora.Application.Interfaces
{
   
    public interface INotificationDispatcher
    {
        
        Task DispatchAsync(
            Guid userId,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default);

       
        Task DispatchToManyAsync(
            IEnumerable<Guid> userIds,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default);

      
        Task<Guid> SaveOnlyAsync(
            Guid userId,
            RealtimeNotificationDto notification,
            CancellationToken cancellationToken = default);
    }
}