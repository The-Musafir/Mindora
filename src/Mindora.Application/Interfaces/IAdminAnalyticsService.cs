using Mindora.Application.DTOs.AdminDashboard;

namespace Mindora.Application.Interfaces
{
    public interface IAdminAnalyticsService
    {
        Task<AdminAnalyticsDto> GetAnalyticsAsync();
    }
}