using Mindora.Application.DTOs.AdminDashboard;

namespace Mindora.Application.Interfaces
{
    public interface IAdminAuditService
    {
        Task<IReadOnlyList<AdminAuditLogDto>> GetAuditLogsAsync();
        Task<IReadOnlyList<AdminLoginHistoryDto>> GetLoginHistoriesAsync();
    }
}