using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Entities;

namespace SkipperData.Managers;

public class VesselManager : ManagerBase<VesselEntity, VesselRepository>
{
    public VesselManager(VesselRepository vesselRepository) : base(vesselRepository) { }
}