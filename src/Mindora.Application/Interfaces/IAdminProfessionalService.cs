using Mindora.Application.DTOs.AdminDashboard;

namespace Mindora.Application.Interfaces
{
    public interface IAdminProfessionalService
    {
        Task<IReadOnlyList<AdminProfessionalDto>> GetAllProfessionalsAsync(string? searchTerm = null);
        Task<AdminProfessionalDto?> GetProfessionalByIdAsync(Guid providerId);
        Task<bool> ToggleVerificationAsync(Guid providerId, bool isVerified);
        Task<bool> ToggleActiveAsync(Guid providerId, bool isActive);
    }
}