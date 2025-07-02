using System.Linq.Expressions;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Data.Repositories;

public interface IRepository<TEntity, TDto> 
where TEntity : class, IEntity<TEntity, TDto>
where TDto : class, IModel<TDto, TEntity>
{
    // Basic CRUD
    Task<TEntity?> GetByIdAsync(long id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(long id);
    
    // Paged fetching with ordering
    Task<int> GetPageCountAsync(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters);
    Task<PagedResult<TEntity>> GetPagedAsync(PagingParameters<TEntity> pagingParameters);
    Task<PagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, PagingParameters<TEntity> pagingParameters);
    
    // Essentials only
    Task<bool> ExistsAsync(long id);
    Task<Result> SaveChangesAsync();
} 