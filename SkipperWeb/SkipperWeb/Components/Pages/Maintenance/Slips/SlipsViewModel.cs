using SkipperModels.Entities;
using SkipperModels.Models;
using SkipperWeb.ApiClients;
using Web.Shared.Maintenance.Entities;

namespace SkipperWeb.Components.Pages.Maintenance.Slips;

public class SlipsViewModel : ModelPageViewModel<SlipModel, SlipEntity, SlipClient, SlipClientConfig>
{
    public SlipsViewModel(SlipClient client) : base(client) { }
}