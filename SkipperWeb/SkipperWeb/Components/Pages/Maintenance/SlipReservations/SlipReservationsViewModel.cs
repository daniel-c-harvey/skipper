using SkipperModels.Models;
using SkipperWeb.ApiClients;
using Web.Shared.Maintenance.Entities;

namespace SkipperWeb.Components.Pages.Maintenance.SlipReservations;

public class SlipReservationsViewModel : ModelPageViewModel<SlipReservationModel, SlipReservationClient, SlipReservationClientConfig>
{
    public SlipReservationsViewModel(SlipReservationClient client) : base(client) { }
} 