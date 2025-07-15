using Microsoft.Extensions.Options;
using System.Text.Json;
using SkipperModels.Models;
using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients;

public class SlipClassificationClient : ModelClient<SlipClassificationModel, SlipClassificationClientConfig>
{
    public SlipClassificationClient(SlipClassificationClientConfig config, IOptions<JsonSerializerOptions> options) 
    : base(config, options) { }
}