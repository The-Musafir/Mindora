using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminUserManagementService : IAdminUserManagementService
    {
        private readonly MindoraDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminUserManagementService(
            MindoraDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IReadOnlyList<AdminUserDto>> GetAllUsersAsync(string? searchTerm = null)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => u.Email.Contains(searchTerm) ||
                                         u.UserName.Contains(searchTerm));
            }

            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();

            var result = new List<AdminUserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new AdminUserDto
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    IsActive = user.IsActive,
                    IsDeleted = user.IsDeleted,
                    Role = roles.FirstOrDefault() ?? "User",
                    CreatedAt = user.CreatedAt
                });
            }

            return result;
        }

        public async Task<AdminUserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new AdminUserDto
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted,
                Role = roles.FirstOrDefault() ?? "User",
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> ToggleUserActiveAsync(Guid userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeUserRoleAsync(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in currentRoles)
                await _userManager.RemoveFromRoleAsync(user, role);

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }
    }
}