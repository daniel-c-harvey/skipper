using NetBlocks.Models;
using Web.Shared.ApiClients;

namespace AuthBlocksWeb.ApiClients;

public class AuthClientConfig : ModelControllerClientConfig
{
    public AuthClientConfig(string baseURL, int port) : base(baseURL, port, "auth")
    {
    }

    public AuthClientConfig(string url) : base(url, "auth")
    {
    }
}