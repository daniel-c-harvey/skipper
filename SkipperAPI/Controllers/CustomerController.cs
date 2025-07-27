using API.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;
using Models.Shared.Converters;
using SkipperData.Data.Repositories;
using SkipperData.Managers;
using SkipperModels;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers;

public class CustomerController<TCustomerEntity, TCustomerModel, TCustomerManager, TRepository, TConverter> : ModelController<TCustomerEntity, TCustomerModel, TCustomerManager>
    where TCustomerEntity : CustomerEntity, new()
    where TCustomerModel : CustomerModel, new()
    where TCustomerManager : CustomerManager<TCustomerEntity, TCustomerModel, TRepository, TConverter>
    where TRepository : ICustomerRepository<TCustomerEntity>
    where TConverter : IEntityToModelConverter<TCustomerEntity, TCustomerModel>
{
    protected readonly TCustomerManager CustomerManager;

    public CustomerController(TCustomerManager manager) : base(manager)
    {
        CustomerManager = manager;
    }

    // Type-specific endpoints (only work with TCustomerEntity)
    [HttpGet("active")]
    public virtual async Task<ActionResult<IEnumerable<TCustomerModel>>> GetActiveCustomers()
    {
        var results = await CustomerManager.GetActiveCustomersAsync();
        return Ok(results);
    }

    [HttpGet("search/{searchTerm}")]
    public virtual async Task<ActionResult<IEnumerable<TCustomerModel>>> SearchCustomers(string searchTerm)
    {
        var results = await CustomerManager.SearchCustomersAsync(searchTerm);
        return Ok(results);
    }
} 