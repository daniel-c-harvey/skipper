using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients;

public class VesselOwnerCustomerClientConfig : ModelClientConfig
{
    public VesselOwnerCustomerClientConfig(string baseURL, int port) : base(baseURL, port, "vesselownercustomer")
    {
    }

    public VesselOwnerCustomerClientConfig(string url) : base(url, "vesselownercustomer")
    {
    }
}