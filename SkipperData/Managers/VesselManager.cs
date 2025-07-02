using SkipperData.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class VesselManager : ManagerBase<VesselEntity, VesselModel>
{
    public VesselManager(IRepository<VesselEntity, VesselModel> vesselRepository) : base(vesselRepository) { }
}