using System.Text.Json;
using Microsoft.Extensions.Options;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperWeb.ApiClients;

public class RentalAgreementClient : ModelControllerClient<RentalAgreementModel, RentalAgreementEntity, RentalAgreementClientConfig>
{
    public RentalAgreementClient(RentalAgreementClientConfig config, IOptions<JsonSerializerOptions> options) : base(config, options) { }
}