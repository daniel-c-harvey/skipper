using System.Linq.Expressions;
using Models.Shared.Common;
using NetBlocks.Models;

namespace Data.Shared.Data.Repositories;

public interface ICrudRepository<TEntity>
{
    // CRUD
    Task<TEntity?> GetByIdAsync(long id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
    
    // Paged fetching with ordering
    Task<int> GetPageCountAsync(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters);
    Task<PagedResult<TEntity>> GetPagedAsync(PagingParameters<TEntity> pagingParameters);
    Task<PagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters);
}