using Mindora.Application.DTOs.Analytics;

namespace Mindora.Application.Interfaces
{
    public interface IAnalyticsEventService
    {
        Task LogEventAsync(CreateAnalyticsEventDto dto);
        Task<IReadOnlyList<AnalyticsEventDto>> GetRecentEventsAsync(int count = 100);
        Task<IReadOnlyList<AnalyticsEventDto>> GetEventsByUserAsync(Guid userId, int count = 100);
        Task<IReadOnlyList<AnalyticsEventDto>> GetEventsByTypeAsync(string eventType, int count = 100);
    }
}