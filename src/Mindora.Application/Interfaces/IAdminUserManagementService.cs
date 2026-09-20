using Mindora.Application.DTOs.AdminDashboard;

namespace Mindora.Application.Interfaces
{
    public interface IAdminUserManagementService
    {
        Task<IReadOnlyList<AdminUserDto>> GetAllUsersAsync(string? searchTerm = null);
        Task<AdminUserDto?> GetUserByIdAsync(Guid userId);
        Task<bool> ToggleUserActiveAsync(Guid userId, bool isActive);
        Task<bool> SoftDeleteUserAsync(Guid userId);
        Task<bool> ChangeUserRoleAsync(Guid userId, string roleName);
    }
}