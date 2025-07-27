using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public interface IVesselOwnerCustomerRepository : ICustomerRepository<VesselOwnerCustomerEntity>
{
    // Vessel owner specific methods
    Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersByLicenseAsync(string licenseNumber);
    Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersWithExpiredLicensesAsync();
    Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersWithVesselsAsync();
}

public class VesselOwnerCustomerRepository : CustomerRepository<VesselOwnerCustomerEntity>, IVesselOwnerCustomerRepository
{
    public VesselOwnerCustomerRepository(SkipperContext context, ILogger<VesselOwnerCustomerRepository> logger) 
        : base(context, logger)
    {
    }

    // Override SearchCustomersAsync to include LicenseNumber in search
    public override async Task<IEnumerable<VesselOwnerCustomerEntity>> SearchCustomersAsync(string searchTerm)
    {
        return await Context.Customers
            .OfType<VesselOwnerCustomerEntity>()
            .Where(c => !c.IsDeleted && 
                (c.Name.Contains(searchTerm) || 
                 c.AccountNumber.Contains(searchTerm) ||
                 (c.LicenseNumber != null && c.LicenseNumber.Contains(searchTerm))))
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersByLicenseAsync(string licenseNumber)
    {
        return await Context.Customers
            .OfType<VesselOwnerCustomerEntity>()
            .Where(v => v.LicenseNumber != null && v.LicenseNumber.Contains(licenseNumber) && !v.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersWithExpiredLicensesAsync()
    {
        var now = DateTime.UtcNow;
        return await Context.Customers
            .OfType<VesselOwnerCustomerEntity>()
            .Where(v => v.LicenseExpiryDate < now && !v.IsDeleted)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersWithVesselsAsync()
    {
        return await Context.Customers
            .OfType<VesselOwnerCustomerEntity>()
            .Where(v => !v.IsDeleted)
            .Include(v => v.VesselOwnerVessels)
                .ThenInclude(vv => vv.Vessel)
            .Where(v => v.VesselOwnerVessels.Any(vv => !vv.Vessel.IsDeleted))
            .ToListAsync();
    }
} 