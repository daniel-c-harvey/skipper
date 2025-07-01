using Microsoft.Extensions.Options;
using NetBlocks.Models;
using SkipperModels.Common;
using SkipperModels.Entities;
using System.Text.Json;

namespace SkipperWeb.ApiClients;

public class VesselClient : ModelControllerClient<VesselModel, VesselEntity, ModelControllerClientConfig>
{
    public VesselClient(VesselClientConfig config, IOptions<JsonSerializerOptions> options) 
    : base(config, options) { }
}