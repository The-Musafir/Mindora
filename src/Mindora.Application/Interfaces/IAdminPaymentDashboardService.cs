using Mindora.Application.DTOs.Payment;

namespace Mindora.Application.Interfaces
{
    public interface IAdminPaymentDashboardService
    {
        Task<AdminPaymentDashboardDto> GetDashboardDataAsync();
    }
}