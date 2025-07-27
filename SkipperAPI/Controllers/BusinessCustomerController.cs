using Microsoft.AspNetCore.Mvc;
using SkipperData.Data.Repositories;
using SkipperData.Managers;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessCustomerController : CustomerController<BusinessCustomerEntity, BusinessCustomerModel, BusinessCustomerManager, IBusinessCustomerRepository, BusinessCustomerConverter>
{
    private readonly BusinessCustomerManager _manager;

    public BusinessCustomerController(BusinessCustomerManager manager) : base(manager)
    {
        _manager = manager;
    }

    // Business-specific endpoints
    [HttpGet("by-name/{businessName}")]
    public virtual async Task<ActionResult<IEnumerable<BusinessCustomerModel>>> GetByBusinessName(string businessName)
    {
        var results = await _manager.GetBusinessCustomersByNameAsync(businessName);
        return Ok(results);
    }

    [HttpGet("by-tax-id/{taxId}")]
    public virtual async Task<ActionResult<BusinessCustomerModel?>> GetByTaxId(string taxId)
    {
        var result = await _manager.GetByTaxIdAsync(taxId);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("with-contacts")]
    public virtual async Task<ActionResult<IEnumerable<BusinessCustomerModel>>> GetWithContacts()
    {
        var results = await _manager.GetBusinessCustomersWithContactsAsync();
        return Ok(results);
    }
} 