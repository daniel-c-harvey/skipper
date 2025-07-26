using Data.Shared.Managers;
using SkipperData.Data.Repositories;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public class VesselManager : Manager<VesselEntity, VesselModel, VesselRepository, VesselEntityToModelConverter>
{
    public VesselManager(VesselRepository vesselRepository) : base(vesselRepository) { }
}