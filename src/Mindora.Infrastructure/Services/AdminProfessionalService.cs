using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminProfessionalService : IAdminProfessionalService
    {
        private readonly MindoraDbContext _context;

        public AdminProfessionalService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<AdminProfessionalDto>> GetAllProfessionalsAsync(string? searchTerm = null)
        {
            var query = _context.ProfessionalProviders
                .Include(p => p.User)
                .Include(p => p.SpecialtyMappings)
                    .ThenInclude(m => m.Specialty)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.User.UserName.Contains(searchTerm) ||
                                         p.User.Email.Contains(searchTerm) ||
                                         p.LicenseNumber.Contains(searchTerm));
            }

            var providers = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();

            return providers.Select(p => new AdminProfessionalDto
            {
                ProviderId = p.ProviderId,
                UserId = p.UserId,
                FullName = p.User.UserName ?? p.User.Email ?? "Provider",
                Email = p.User.Email ?? "",
                LicenseNumber = p.LicenseNumber,
                YearsOfExperience = p.YearsOfExperience,
                IsVerified = p.IsVerified,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                Specialties = p.SpecialtyMappings.Select(m => m.Specialty.Name).ToList()
            }).ToList();
        }

        public async Task<AdminProfessionalDto?> GetProfessionalByIdAsync(Guid providerId)
        {
            var p = await _context.ProfessionalProviders
                .Include(x => x.User)
                .Include(x => x.SpecialtyMappings)
                    .ThenInclude(m => m.Specialty)
                .FirstOrDefaultAsync(x => x.ProviderId == providerId);

            if (p == null) return null;

            return new AdminProfessionalDto
            {
                ProviderId = p.ProviderId,
                UserId = p.UserId,
                FullName = p.User.UserName ?? p.User.Email ?? "Provider",
                Email = p.User.Email ?? "",
                LicenseNumber = p.LicenseNumber,
                YearsOfExperience = p.YearsOfExperience,
                IsVerified = p.IsVerified,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                Specialties = p.SpecialtyMappings.Select(m => m.Specialty.Name).ToList()
            };
        }

        public async Task<bool> ToggleVerificationAsync(Guid providerId, bool isVerified)
        {
            var provider = await _context.ProfessionalProviders.FindAsync(providerId);
            if (provider == null) return false;

            provider.IsVerified = isVerified;
            provider.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleActiveAsync(Guid providerId, bool isActive)
        {
            var provider = await _context.ProfessionalProviders.FindAsync(providerId);
            if (provider == null) return false;

            provider.IsActive = isActive;
            provider.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}