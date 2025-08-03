using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public interface IBusinessCustomerRepository : ICustomerRepository<BusinessCustomerEntity>
{
    // Business customer specific methods
    Task<IEnumerable<BusinessCustomerEntity>> GetBusinessCustomersByNameAsync(string businessName);
    Task<BusinessCustomerEntity?> GetByTaxIdAsync(string taxId);
    Task<IEnumerable<BusinessCustomerEntity>> GetBusinessCustomersWithContactsAsync();
}

public class BusinessCustomerRepository : CustomerRepository<BusinessCustomerEntity>, IBusinessCustomerRepository
{
    public BusinessCustomerRepository(SkipperContext context, ILogger<BusinessCustomerRepository> logger) 
        : base(context, logger)
    {
    }

    // Override SearchCustomersAsync to include BusinessName in search
    public override async Task<IEnumerable<BusinessCustomerEntity>> SearchCustomersAsync(string searchTerm)
    {
        return await Query
            .Where(c =>
                (c.Name.Contains(searchTerm) || 
                 c.AccountNumber.Contains(searchTerm) ||
                 c.BusinessName.Contains(searchTerm)))
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<BusinessCustomerEntity>> GetBusinessCustomersByNameAsync(string businessName)
    {
        return await Query
            .Where(b => b.BusinessName.Contains(businessName))
            .ToListAsync();
    }

    public virtual async Task<BusinessCustomerEntity?> GetByTaxIdAsync(string taxId)
    {
        return await Query
            .Where(b => b.TaxId == taxId)
            .FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<BusinessCustomerEntity>> GetBusinessCustomersWithContactsAsync()
    {
        return await Query
            .Include(b => b.BusinessCustomerContacts)
                .ThenInclude(bc => bc.Contact)
                    .ThenInclude(c => c.Address)
            .Where(b => b.BusinessCustomerContacts.Any())
            .ToListAsync();
    }
} 