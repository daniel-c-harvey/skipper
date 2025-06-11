using System.Linq.Expressions;
using Skipper.Domain.Entities;

namespace Skipper.Data.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    // Basic CRUD
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(long id);
    
    // Essentials only
    Task<bool> ExistsAsync(long id);
    Task<int> SaveChangesAsync();
} 