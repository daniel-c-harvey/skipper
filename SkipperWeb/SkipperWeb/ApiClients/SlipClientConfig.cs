namespace SkipperWeb.ApiClients;

public class SlipClientConfig : ModelControllerClientConfig
{
    public SlipClientConfig(string baseURL) : base(baseURL, "slip") { }
}