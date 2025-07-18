﻿using SkipperModels.Models;
using SkipperWeb.ApiClients;
using Web.Shared.Maintenance.Entities;

namespace SkipperWeb.Components.Pages.Maintenance.Vessels;

public class VesselsViewModel : ModelPageViewModel<VesselModel, VesselClient, VesselClientConfig>
{
    public VesselsViewModel(VesselClient client) : base(client) { }
}