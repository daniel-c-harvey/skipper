using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetBlocks.Models;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public interface IVesselOwnerCustomerRepository : ICustomerRepository<VesselOwnerCustomerEntity>
{
    // Vessel owner specific methods
    Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersByLicenseAsync(string licenseNumber);
    Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersWithExpiredLicensesAsync();
    Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersWithVesselsAsync();
    Task<Result> AddVesselToOwner(VesselOwnerCustomerEntity owner, VesselEntity vessel);
}

public class VesselOwnerCustomerRepository : CustomerRepository<VesselOwnerCustomerEntity>, IVesselOwnerCustomerRepository
{
    protected DbSet<VesselOwnerVesselEntity> VesselOwnerVessels;
    
    public VesselOwnerCustomerRepository(SkipperContext context, ILogger<VesselOwnerCustomerRepository> logger) 
        : base(context, logger)
    {
        VesselOwnerVessels = context.Set<VesselOwnerVesselEntity>();
    }

    // Override SearchCustomersAsync to include LicenseNumber in search
    public override async Task<IEnumerable<VesselOwnerCustomerEntity>> SearchCustomersAsync(string searchTerm)
    {
        return await Query
            .Where(c =>  
                (c.Name.Contains(searchTerm) || 
                 c.AccountNumber.Contains(searchTerm) ||
                 (c.LicenseNumber != null && c.LicenseNumber.Contains(searchTerm))))
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersByLicenseAsync(string licenseNumber)
    {
        return await Query
            .Where(v => v.LicenseNumber != null && v.LicenseNumber.Contains(licenseNumber))
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersWithExpiredLicensesAsync()
    {
        var now = DateTime.UtcNow;
        return await Query
            .Where(v => v.LicenseExpiryDate < now)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<VesselOwnerCustomerEntity>> GetVesselOwnersWithVesselsAsync()
    {
        return await Query
            .Include(v => v.VesselOwnerVessels)
                .ThenInclude(vv => vv.Vessel)
            .Where(v => v.VesselOwnerVessels.Any(vv => !vv.Vessel.IsDeleted))
            .ToListAsync();
    }

    public async Task<Result> AddVesselToOwner(VesselOwnerCustomerEntity owner, VesselEntity vessel)
    {
        var now = DateTime.UtcNow;
        var link = await VesselOwnerVessels.AddAsync(new VesselOwnerVesselEntity
        {
            VesselId = vessel.Id,
            VesselOwnerCustomerId = owner.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false,
        });
        await SaveChangesAsync();
        
        return (await VesselOwnerVessels
            .AnyAsync(vv => !vv.IsDeleted &&
                            vv.VesselId == vessel.Id &&
                            vv.VesselOwnerCustomerId == owner.Id))! 
            ? Result.CreatePassResult()
            : Result.CreateFailResult("Vessel could not be linked to owner");
    }
} 