using System.Linq.Expressions;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    // Basic CRUD
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(long id);
    
    // Paged fetching with ordering
    Task<int> GetPageCountAsync(Expression<Func<T, bool>> predicate, PagingParameters<T> pagingParameters);
    Task<PagedResult<T>> GetPagedAsync(PagingParameters<T> pagingParameters);
    Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, PagingParameters<T> pagingParameters);
    
    // Essentials only
    Task<bool> ExistsAsync(long id);
    Task<Result> SaveChangesAsync();
} 