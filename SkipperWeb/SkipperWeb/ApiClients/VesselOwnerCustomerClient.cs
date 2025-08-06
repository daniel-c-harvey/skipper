using System.Text.Json;
using Microsoft.Extensions.Options;
using SkipperModels.InputModels;
using SkipperModels.Models;
using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients;

public class VesselOwnerCustomerClient : ModelClient<VesselOwnerCustomerModel, VesselOwnerCustomerClientConfig>
{
    public VesselOwnerCustomerClient(VesselOwnerCustomerClientConfig config, IOptions<JsonSerializerOptions> options) : base(config, options)
    {
    }
}