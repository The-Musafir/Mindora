using Mindora.Application.DTOs.AdminDashboard;

namespace Mindora.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardOverviewDto> GetOverviewAsync();
    }
}