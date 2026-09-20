using Mindora.Application.DTOs.Analytics;

namespace Mindora.Application.Interfaces
{
    public interface IAdminAnalyticsFullService
    {
        Task<AdminAnalyticsFullDto> GetFullAnalyticsAsync();
    }
}