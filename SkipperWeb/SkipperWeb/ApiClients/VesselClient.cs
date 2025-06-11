using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;

namespace SkipperWeb.ApiClients;

public class VesselClient : EntityControllerClient<Vessel, EntityControllerClientConfig>
{
    public VesselClient(EntityControllerClientConfig config) : base(config) { } // TODO register the client and test the API
}