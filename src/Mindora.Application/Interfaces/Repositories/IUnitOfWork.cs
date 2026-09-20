namespace Mindora.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }

    IRoleRepository Roles { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}