using Mindora.Domain.Entities;

namespace Mindora.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetUserWithRolesAsync(Guid userId);

    Task<User?> GetUserWithProfileAsync(Guid userId);

    Task<bool> IsEmailExistsAsync(string email);
}