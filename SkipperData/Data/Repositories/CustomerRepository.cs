using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Shared.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels;
using System.Linq.Expressions;
using Models.Shared.Common;
using NetBlocks.Models;

namespace SkipperData.Data.Repositories;

// Generic repository for type-specific customer operations
public class CustomerRepository<TCustomerEntity> : Repository<SkipperContext, TCustomerEntity>, ICustomerRepository<TCustomerEntity>
    where TCustomerEntity : CustomerEntity
{
    public CustomerRepository(SkipperContext context, ILogger<CustomerRepository<TCustomerEntity>> logger) 
        : base(context, logger, q => q.OfType<TCustomerEntity>())
    {
    }
    
    #region ICustomerRepository<TCustomerEntity> Implementation (Type-Specific Business Methods)
    public virtual async Task<IEnumerable<TCustomerEntity>> SearchCustomersAsync(string searchTerm)
    {
        return await Query
            .Where(c => 
                (c.Name.Contains(searchTerm) || 
                 c.AccountNumber.Contains(searchTerm)))
            .ToListAsync();
    }

    #endregion
}

// Base non-generic repository for cross-type customer operations
// public class CustomerRepository : RepositoryBase<SkipperContext, CustomerEntity>, ICustomerRepository
// {
//     public CustomerRepository(SkipperContext context, ILogger<CustomerRepository> logger) 
//         : base(context, logger)
//     {
//     }
//
//     #region ICrudRepository<CustomerEntity> Implementation
//
//     public virtual async Task<CustomerEntity?> GetByIdAsync(long id)
//     {
//         return await Context.Customers
//             .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
//     }
//
//     public virtual async Task<IEnumerable<CustomerEntity>> GetAllAsync()
//     {
//         return await Context.Customers
//             .Where(c => !c.IsDeleted)
//             .ToListAsync();
//     }
//
//     public virtual async Task<IEnumerable<CustomerEntity>> FindAsync(Expression<Func<CustomerEntity, bool>> predicate)
//     {
//         return await Context.Customers
//             .Where(c => !c.IsDeleted)
//             .Where(predicate)
//             .ToListAsync();
//     }
//
//     public virtual async Task<CustomerEntity> AddAsync(CustomerEntity entity)
//     {
//         entity.CreatedAt = DateTime.UtcNow;
//         entity.UpdatedAt = DateTime.UtcNow;
//         
//         var result = await Context.Customers.AddAsync(entity);
//         await Context.SaveChangesAsync();
//         return result.Entity;
//     }
//
//     public virtual async Task UpdateAsync(CustomerEntity entity)
//     {
//         entity.UpdatedAt = DateTime.UtcNow;
//         Context.Customers.Update(entity);
//         await Context.SaveChangesAsync();
//     }
//
//     public virtual async Task DeleteAsync(long id)
//     {
//         var entity = await GetByIdAsync(id);
//         if (entity != null)
//         {
//             entity.IsDeleted = true;
//             entity.UpdatedAt = DateTime.UtcNow;
//             await Context.SaveChangesAsync();
//         }
//     }
//
//     public virtual async Task<bool> ExistsAsync(long id)
//     {
//         return await Context.Customers
//             .AnyAsync(c => c.Id == id && !c.IsDeleted);
//     }
//
//     public virtual async Task<int> GetPageCountAsync(Expression<Func<CustomerEntity, bool>> predicate, PagingParameters<CustomerEntity> pagingParameters)
//     {
//         var query = Context.Customers
//             .Where(c => !c.IsDeleted)
//             .Where(predicate);
//             
//         double rowCount = (double)(await query.CountAsync()) / pagingParameters.PageSize;
//         return (int)Math.Ceiling(rowCount);
//     }
//
//     public virtual async Task<PagedResult<CustomerEntity>> GetPagedAsync(PagingParameters<CustomerEntity> pagingParameters)
//     {
//         var query = Context.Customers
//             .Where(c => !c.IsDeleted);
//                 
//         return await ExecutePagedQueryAsync(query, pagingParameters);
//     }
//
//     public virtual async Task<PagedResult<CustomerEntity>> GetPagedAsync(Expression<Func<CustomerEntity, bool>> predicate, PagingParameters<CustomerEntity> pagingParameters)
//     {
//         var query = Context.Customers
//             .Where(c => !c.IsDeleted)
//             .Where(predicate);
//                 
//         return await ExecutePagedQueryAsync(query, pagingParameters);
//     }
//
//     #endregion
//
//     #region ICustomerRepository Implementation (Cross-Type Business Methods)
//
//     public virtual async Task<CustomerEntity?> GetByAccountNumberAsync(string accountNumber)
//     {
//         return await Context.Customers
//             .FirstOrDefaultAsync(c => c.AccountNumber == accountNumber && !c.IsDeleted);
//     }
//
//     public virtual Task<CustomerEntity?> GetByEmailAsync(string email)
//     {
//         // NOTE: Email is not on abstract CustomerEntity, only on derived types through Contact
//         // This method would need to query derived types or be redesigned
//         throw new NotImplementedException("Email search requires specific customer type since Contact is on derived classes");
//     }
//
//     public virtual async Task<IEnumerable<CustomerEntity>> GetCustomersByTypeAsync(CustomerProfileType customerType)
//     {
//         return await Context.Customers
//             .Where(c => c.CustomerProfileType == customerType && !c.IsDeleted)
//             .ToListAsync();
//     }
//
//     public virtual async Task<IEnumerable<CustomerEntity>> GetCustomersByTypesAsync(params CustomerProfileType[] customerTypes)
//     {
//         return await Context.Customers
//             .Where(c => customerTypes.Contains(c.CustomerProfileType) && !c.IsDeleted)
//             .ToListAsync();
//     }
//
//     public virtual async Task<PagedResult<CustomerEntity>> GetCustomersPagedAsync(
//         Expression<Func<CustomerEntity, bool>>? predicate = null,
//         PagingParameters<CustomerEntity>? pagingParameters = null)
//     {
//         var query = Context.Customers
//             .Where(c => !c.IsDeleted);
//
//         if (predicate != null)
//             query = query.Where(predicate);
//
//         if (pagingParameters != null)
//             return await ExecutePagedQueryAsync(query, pagingParameters);
//         else
//         {
//             var allResults = await query.ToListAsync();
//             return new PagedResult<CustomerEntity>
//             {
//                 Items = allResults,
//                 TotalCount = allResults.Count,
//                 Page = 1,
//                 PageSize = allResults.Count
//             };
//         }
//     }
//
//     public virtual async Task<IEnumerable<CustomerEntity>> GetActiveCustomersAsync()
//     {
//         return await Context.Customers
//             .Where(c => !c.IsDeleted)
//             .ToListAsync();
//     }
//
//     public virtual async Task<IEnumerable<CustomerEntity>> SearchCustomersAsync(string searchTerm)
//     {
//         return await Context.Customers
//             .Where(c => !c.IsDeleted && 
//                 (c.Name.Contains(searchTerm) || 
//                  c.AccountNumber.Contains(searchTerm)))
//             .ToListAsync();
//     }
//
//     public virtual async Task<int> GetCustomerCountByTypeAsync(CustomerProfileType customerType)
//     {
//         return await Context.Customers
//             .CountAsync(c => c.CustomerProfileType == customerType && !c.IsDeleted);
//     }
//
//     #endregion
// } 