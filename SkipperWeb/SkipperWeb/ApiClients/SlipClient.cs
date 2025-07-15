using Microsoft.Extensions.Options;
using System.Text.Json;
using SkipperModels.Models;
using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients;

public class SlipClient : ModelClient<SlipModel, SlipClientConfig>
{
    public SlipClient(SlipClientConfig config, IOptions<JsonSerializerOptions> options) 
    : base(config, options) { }
}