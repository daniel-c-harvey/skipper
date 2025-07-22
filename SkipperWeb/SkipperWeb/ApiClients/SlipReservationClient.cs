using System.Text.Json;
using Microsoft.Extensions.Options;
using SkipperModels.Entities;
using SkipperModels.Models;
using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients;

public class SlipReservationClient : ModelClient<SlipReservationModel, SlipReservationClientConfig>
{
    public SlipReservationClient(SlipReservationClientConfig config, IOptions<JsonSerializerOptions> options) : base(config, options) { }
} 