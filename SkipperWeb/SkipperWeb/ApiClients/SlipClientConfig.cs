using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients;

public class SlipClientConfig : ModelClientConfig
{
    public SlipClientConfig(string baseURL) : base(baseURL, "slip") { }
}