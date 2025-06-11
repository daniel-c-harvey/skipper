using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperWeb.ApiClients;

public class VesselClient : EntityControllerClient<Vessel, EntityControllerClientConfig>
{
    public VesselClient(VesselClientConfig config) : base(config) { }
}