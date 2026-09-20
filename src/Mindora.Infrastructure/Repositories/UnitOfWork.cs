using Mindora.Application.Interfaces.Repositories;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly MindoraDbContext _context;

    public IUserRepository Users { get; }

    public IRoleRepository Roles { get; }

    public UnitOfWork(MindoraDbContext context)
    {
        _context = context;

        Users = new UserRepository(context);
        Roles = new RoleRepository(context);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}