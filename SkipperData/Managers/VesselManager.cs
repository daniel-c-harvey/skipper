using Data.Shared.Data.Repositories;
using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class VesselManager : ManagerBase<VesselEntity, VesselModel, VesselRepository>
{
    public VesselManager(VesselRepository vesselRepository) : base(vesselRepository) { }
}