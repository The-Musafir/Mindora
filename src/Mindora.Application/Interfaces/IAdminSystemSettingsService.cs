using Mindora.Application.DTOs.AdminDashboard;

namespace Mindora.Application.Interfaces
{
    public interface IAdminSystemSettingsService
    {
        Task<AdminSystemSettingsDto> GetSettingsAsync();
        Task<bool> UpdateSettingsAsync(AdminSystemSettingsDto dto);
    }
}