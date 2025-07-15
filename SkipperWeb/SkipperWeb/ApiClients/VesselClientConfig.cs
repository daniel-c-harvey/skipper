using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients
{
    public class VesselClientConfig : ModelClientConfig
    {
        public VesselClientConfig(string baseURL) : base(baseURL, "vessel") { }
    }
}
