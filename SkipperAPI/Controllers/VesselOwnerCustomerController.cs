using Microsoft.AspNetCore.Mvc;
using SkipperData.Data.Repositories;
using SkipperData.Managers;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VesselOwnerCustomerController : CustomerController<VesselOwnerCustomerEntity, VesselOwnerCustomerModel, VesselOwnerCustomerManager, IVesselOwnerCustomerRepository, VesselOwnerCustomerConverter>
{
    private readonly VesselOwnerCustomerManager _manager;

    public VesselOwnerCustomerController(VesselOwnerCustomerManager manager) : base(manager)
    {
        _manager = manager;
    }

    // VesselOwner-specific endpoints
    [HttpGet("by-license/{licenseNumber}")]
    public virtual async Task<ActionResult<IEnumerable<VesselOwnerCustomerModel>>> GetByLicenseNumber(string licenseNumber)
    {
        var results = await _manager.GetVesselOwnersByLicenseAsync(licenseNumber);
        return Ok(results);
    }

    [HttpGet("expired-licenses")]
    public virtual async Task<ActionResult<IEnumerable<VesselOwnerCustomerModel>>> GetWithExpiredLicenses()
    {
        var results = await _manager.GetVesselOwnersWithExpiredLicensesAsync();
        return Ok(results);
    }

    [HttpGet("with-vessels")]
    public virtual async Task<ActionResult<IEnumerable<VesselOwnerCustomerModel>>> GetWithVessels()
    {
        var results = await _manager.GetVesselOwnersWithVesselsAsync();
        return Ok(results);
    }
} 