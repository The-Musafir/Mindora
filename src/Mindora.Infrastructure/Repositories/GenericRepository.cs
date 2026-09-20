using Microsoft.EntityFrameworkCore;
using Mindora.Application.Interfaces.Repositories;
using Mindora.Infrastructure.Persistence.DbContext;
using System.Linq.Expressions;

namespace Mindora.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    protected readonly MindoraDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(MindoraDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate)
    {
        return await _dbSet
            .Where(predicate)
            .ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity is not null;
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null)
    {
        return predicate == null
            ? await _dbSet.CountAsync()
            : await _dbSet.CountAsync(predicate);
    }
}