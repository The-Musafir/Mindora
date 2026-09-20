using Mindora.Application.DTOs.Analytics;

namespace Mindora.Application.Interfaces
{
    public interface IUserAnalyticsService
    {
        Task<UserAnalyticsDto> GetUserAnalyticsAsync(Guid userId);
    }
}