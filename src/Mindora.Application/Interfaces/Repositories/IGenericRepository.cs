using System.Linq.Expressions;

namespace Mindora.Application.Interfaces.Repositories;

public interface IGenericRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<TEntity>> GetAllAsync();

    Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate);

    Task AddAsync(TEntity entity);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    Task<bool> ExistsAsync(Guid id);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null);
}