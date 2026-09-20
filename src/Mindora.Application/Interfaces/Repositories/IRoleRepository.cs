using Mindora.Domain.Entities;

namespace Mindora.Application.Interfaces.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByNameAsync(string roleName);

    Task<Role?> GetByNormalizedNameAsync(string normalizedName);

    Task<bool> IsRoleExistsAsync(string roleName);
}