using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class BusinessCustomerManager : CustomerManager<BusinessCustomerEntity, BusinessCustomerModel, IBusinessCustomerRepository, BusinessCustomerConverter>
{
    public BusinessCustomerManager(IBusinessCustomerRepository repository) : base(repository)
    {
    }

    // Business customer specific business logic methods
    public virtual async Task<IEnumerable<BusinessCustomerModel>> GetBusinessCustomersByNameAsync(string businessName)
    {
        var entities = await Repository.GetBusinessCustomersByNameAsync(businessName);
        return entities.Select(BusinessCustomerConverter.Convert);
    }

    public virtual async Task<BusinessCustomerModel?> GetByTaxIdAsync(string taxId)
    {
        var entity = await Repository.GetByTaxIdAsync(taxId);
        return entity != null ? BusinessCustomerConverter.Convert(entity) : null;
    }

    public virtual async Task<IEnumerable<BusinessCustomerModel>> GetBusinessCustomersWithContactsAsync()
    {
        var entities = await Repository.GetBusinessCustomersWithContactsAsync();
        return entities.Select(BusinessCustomerConverter.Convert);
    }
} 