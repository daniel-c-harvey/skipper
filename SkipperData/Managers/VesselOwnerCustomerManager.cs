using Data.Shared.Managers;
using NetBlocks.Models;
using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class VesselOwnerCustomerManager : CustomerManager<VesselOwnerCustomerEntity, VesselOwnerCustomerModel, IVesselOwnerCustomerRepository, VesselOwnerCustomerConverter>
{
    private readonly ContactManager _contactManager;
    private readonly VesselManager _vesselManager;

    public VesselOwnerCustomerManager(IVesselOwnerCustomerRepository repository, ContactManager contactManager, VesselManager vesselManager) : base(repository)
    {
        _contactManager = contactManager;
        _vesselManager = vesselManager;
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

    public virtual async Task<ResultContainer<VesselModel>> AddVesselToOwner(VesselOwnerCustomerModel owner, VesselModel vessel)
    {
        try
        {
            if (await _vesselManager.Exists(vessel) is { Value: false })
            {
                var result = await _vesselManager.Add(vessel);
                if (result is { Value: VesselModel newVessel })
                {
                    vessel = newVessel;
                }
            }
            
            var addResult = await Repository.AddVesselToOwner(
                VesselOwnerCustomerConverter.Convert(owner),
                VesselEntityToModelConverter.Convert(vessel));
            
            return ResultContainer<VesselModel>.From(addResult, vessel);
        }
        catch (Exception e)
        {
            return ResultContainer<VesselModel>.CreateFailResult(e.Message);
        }
    }
} 