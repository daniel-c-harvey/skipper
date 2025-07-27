using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class VesselOwnerCustomerManager : CustomerManager<VesselOwnerCustomerEntity, VesselOwnerCustomerModel, IVesselOwnerCustomerRepository, VesselOwnerCustomerConverter>
{
    public VesselOwnerCustomerManager(IVesselOwnerCustomerRepository repository) : base(repository)
    {
    }

    // Vessel owner specific business logic methods
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