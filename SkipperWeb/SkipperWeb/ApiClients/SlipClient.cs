using Microsoft.Extensions.Options;
using SkipperModels.Entities;
using System.Text.Json;

namespace SkipperWeb.ApiClients;

public class SlipClient : ModelControllerClient<SlipModel, SlipEntity, SlipClientConfig>
{
    public SlipClient(SlipClientConfig config, IOptions<JsonSerializerOptions> options) 
    : base(config, options) { }
}