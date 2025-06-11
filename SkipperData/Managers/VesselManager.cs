using SkipperData.Data.Repositories;
using SkipperModels.Entities;

namespace SkipperData.Managers;

public class VesselManager : ManagerBase<Vessel>
{
    public VesselManager(IRepository<Vessel> vesselRepository) : base(vesselRepository) { }
}