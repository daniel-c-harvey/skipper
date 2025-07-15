using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients;

public class SlipClassificationClientConfig : ModelClientConfig
{
    public SlipClassificationClientConfig(string baseURL, int port) : base(baseURL, port, "slipclassification") { }

    public SlipClassificationClientConfig(string url) : base(url, "slipclassification") { }
}