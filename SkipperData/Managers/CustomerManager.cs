using Data.Shared.Managers;
using Models.Shared.Converters;
using SkipperData.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class CustomerManager<TCustomerEntity, TCustomerModel, TRepository, TConverter> 
    : Manager<TCustomerEntity, TCustomerModel, TRepository, TConverter>
    where TCustomerEntity : CustomerEntity, new()
    where TCustomerModel : CustomerModel, new()
    where TRepository : ICustomerRepository<TCustomerEntity>
    where TConverter : IEntityToModelConverter<TCustomerEntity, TCustomerModel>
{
    protected CustomerManager(TRepository repository) : base(repository)
    {
    }

    public virtual async Task<IEnumerable<TCustomerModel>> SearchCustomersAsync(string searchTerm)
    {
        var entities = await Repository.SearchCustomersAsync(searchTerm);
        return entities.Select(TConverter.Convert);
    }
} 