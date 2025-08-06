using SkipperModels.Models;
using SkipperWeb.ApiClients;
using Web.Shared.Maintenance.Entities;

namespace SkipperWeb.Components.Pages.Maintenance.Customers.VesselOwners;

public class VesselOwnerCustomersViewModel : ModelPageViewModel<VesselOwnerCustomerModel, VesselOwnerCustomerClient, VesselOwnerCustomerClientConfig>
{
    public VesselOwnerCustomersViewModel(VesselOwnerCustomerClient client) : base(client)
    {
    }
}