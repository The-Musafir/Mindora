using Mindora.Application.DTOs.Payment;

namespace Mindora.Application.Interfaces
{
    public interface IRevenueService
    {
        Task<RevenueAnalyticsDto> GetRevenueAnalyticsAsync();
    }
}