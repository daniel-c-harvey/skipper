using Microsoft.Extensions.Options;
using System.Text.Json;
using SkipperModels.Models;
using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients;

public class VesselClient : ModelClient<VesselModel, VesselClientConfig>
{
    public VesselClient(VesselClientConfig config, IOptions<JsonSerializerOptions> options) 
    : base(config, options) { }
}