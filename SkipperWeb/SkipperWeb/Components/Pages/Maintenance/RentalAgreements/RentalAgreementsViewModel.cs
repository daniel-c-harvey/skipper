using SkipperModels.Entities;
using SkipperModels.Models;
using SkipperWeb.ApiClients;
using Web.Shared.Maintenance.Entities;

namespace SkipperWeb.Components.Pages.Maintenance.RentalAgreements;

public class RentalAgreementsViewModel : ModelPageViewModel<RentalAgreementModel, RentalAgreementEntity, RentalAgreementClient, RentalAgreementClientConfig>
{
    public RentalAgreementsViewModel(RentalAgreementClient client) : base(client) { }
}