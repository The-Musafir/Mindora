using Microsoft.EntityFrameworkCore;
using Mindora.Application.Interfaces;
using Mindora.Application.Interfaces.Repositories;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MindoraDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email.ToLower());
        }

        public async Task<User?> GetUserWithRolesAsync(Guid userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithProfileAsync(Guid userId)
        {
            return await _dbSet
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email.ToLower());
        }
    }
}