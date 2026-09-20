using Mindora.Application.DTOs.AdminDashboard;

namespace Mindora.Application.Interfaces
{
    public interface IAdminModerationService
    {
        Task<IReadOnlyList<AdminModerationFlagDto>> GetPendingFlagsAsync();
        Task<IReadOnlyList<AdminModerationFlagDto>> GetResolvedFlagsAsync();
        Task<AdminModerationFlagDto?> GetFlagByIdAsync(Guid flagId);
        Task<bool> ApproveFlagAsync(Guid flagId, Guid adminUserId);
        Task<bool> DismissFlagAsync(Guid flagId, Guid adminUserId);
    }
}