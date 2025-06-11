using Skipper.Data.Repositories;
using SkipperModels.Entities;

namespace Skipper.Managers;

public class VesselManager : ManagerBase<Vessel>
{
    public VesselManager(IRepository<Vessel> vesselRepository) : base(vesselRepository) { }
}