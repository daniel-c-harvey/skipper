using Microsoft.Extensions.Options;
using SkipperModels.Entities;
using System.Text.Json;

namespace SkipperWeb.ApiClients;

public class SlipClassificationClient : ModelControllerClient<SlipClassificationModel, SlipClassificationEntity, SlipClassificationClientConfig>
{
    public SlipClassificationClient(SlipClassificationClientConfig config, IOptions<JsonSerializerOptions> options) 
    : base(config, options) { }
}