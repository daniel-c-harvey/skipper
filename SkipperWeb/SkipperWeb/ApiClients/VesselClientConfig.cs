using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients
{
    public class VesselClientConfig : ModelControllerClientConfig
    {
        public VesselClientConfig(string baseURL) : base(baseURL, "vessel") { }
    }
}
