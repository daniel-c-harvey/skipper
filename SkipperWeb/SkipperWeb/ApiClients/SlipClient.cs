using Microsoft.Extensions.Options;
using SkipperModels.Entities;
using System.Text.Json;
using SkipperModels.Models;

namespace SkipperWeb.ApiClients;

public class SlipClient : ModelControllerClient<SlipModel, SlipEntity, SlipClientConfig>
{
    public SlipClient(SlipClientConfig config, IOptions<JsonSerializerOptions> options) 
    : base(config, options) { }
}