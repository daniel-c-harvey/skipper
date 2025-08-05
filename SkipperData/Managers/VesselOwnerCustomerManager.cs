using NetBlocks.Models;
using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class VesselOwnerCustomerManager : CustomerManager<VesselOwnerCustomerEntity, VesselOwnerCustomerModel, IVesselOwnerCustomerRepository, VesselOwnerCustomerConverter>
{
    private readonly ContactManager _contactManager;

    public VesselOwnerCustomerManager(IVesselOwnerCustomerRepository repository, ContactManager contactManager) : base(repository)
    {
        _contactManager = contactManager;
    }

    public override async Task<ResultContainer<VesselOwnerCustomerModel>> Add(VesselOwnerCustomerModel entity)
    {
        if (await _contactManager.Exists(entity.Contact) is {Value: false})
        {
            var result = await _contactManager.Add(entity.Contact);
            if (result is {Success: true, Value: ContactModel contact})
            {
                entity.Contact = contact;
            }
        }
        
        return await base.Add(entity);
    }

    public virtual async Task<IEnumerable<VesselOwnerCustomerModel>> GetVesselOwnersByLicenseAsync(string licenseNumber)
    {
        var entities = await Repository.GetVesselOwnersByLicenseAsync(licenseNumber);
        return entities.Select(VesselOwnerCustomerConverter.Convert);
    }

    public virtual async Task<IEnumerable<VesselOwnerCustomerModel>> GetVesselOwnersWithExpiredLicensesAsync()
    {
        var entities = await Repository.GetVesselOwnersWithExpiredLicensesAsync();
        return entities.Select(VesselOwnerCustomerConverter.Convert);
    }

    public virtual async Task<IEnumerable<VesselOwnerCustomerModel>> GetVesselOwnersWithVesselsAsync()
    {
        var entities = await Repository.GetVesselOwnersWithVesselsAsync();
        return entities.Select(VesselOwnerCustomerConverter.Convert);
    }
} 