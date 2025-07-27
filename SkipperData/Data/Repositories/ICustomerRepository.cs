using System.Linq.Expressions;
using Data.Shared.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels;
using Models.Shared.Common;
using NetBlocks.Models;

namespace SkipperData.Data.Repositories;

// Base interface for cross-type customer operations (works with mixed CustomerEntity types)
public interface ICustomerRepository : IRepository<CustomerEntity>
{
    // Cross-type customer methods that return CustomerEntity (base type)
    Task<CustomerEntity?> GetByAccountNumberAsync(string accountNumber);
    Task<CustomerEntity?> GetByEmailAsync(string email);
    Task<IEnumerable<CustomerEntity>> GetCustomersByTypeAsync(CustomerProfileType customerType);
    Task<IEnumerable<CustomerEntity>> GetCustomersByTypesAsync(params CustomerProfileType[] customerTypes);
    
    // Multi-type queries
    Task<PagedResult<CustomerEntity>> GetCustomersPagedAsync(
        Expression<Func<CustomerEntity, bool>>? predicate = null,
        PagingParameters<CustomerEntity>? pagingParameters = null);
        
    // Business logic methods
    Task<IEnumerable<CustomerEntity>> GetActiveCustomersAsync();
    Task<IEnumerable<CustomerEntity>> SearchCustomersAsync(string searchTerm);
    Task<int> GetCustomerCountByTypeAsync(CustomerProfileType customerType);
}

// Generic interface for type-specific customer operations (works with specific TCustomerEntity only)
public interface ICustomerRepository<TCustomerEntity> : IRepository<TCustomerEntity>
    where TCustomerEntity : CustomerEntity
{
    // Type-specific business methods that return TCustomerEntity only
    // NO cross-type methods here - those belong in ICustomerRepository
    Task<IEnumerable<TCustomerEntity>> GetActiveCustomersAsync();
    Task<IEnumerable<TCustomerEntity>> SearchCustomersAsync(string searchTerm);
} 