using Microsoft.Extensions.Options;
using SkipperModels.Entities;
using System.Text.Json;
using SkipperModels.Models;

namespace SkipperWeb.ApiClients;

public class VesselClient : ModelControllerClient<VesselModel, VesselEntity, VesselClientConfig>
{
    public VesselClient(VesselClientConfig config, IOptions<JsonSerializerOptions> options) 
    : base(config, options) { }
}