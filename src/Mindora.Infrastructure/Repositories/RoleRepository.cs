using Microsoft.EntityFrameworkCore;
using Mindora.Application.Interfaces.Repositories;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(MindoraDbContext context)
        : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }

    public async Task<Role?> GetByNormalizedNameAsync(string normalizedName)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedName);
    }

    public async Task<bool> IsRoleExistsAsync(string roleName)
    {
        return await _dbSet
            .AnyAsync(r => r.Name == roleName);
    }
}