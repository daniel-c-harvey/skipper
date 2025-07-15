using SkipperModels.Models;
using SkipperWeb.ApiClients;
using Web.Shared.Maintenance.Entities;

namespace SkipperWeb.Components.Pages.Maintenance.SlipClassifications;

public class SlipClassificationsViewModel : ModelPageViewModel<SlipClassificationModel, SlipClassificationClient, SlipClassificationClientConfig>
{
    public SlipClassificationsViewModel(SlipClassificationClient client) : base(client) { }
}