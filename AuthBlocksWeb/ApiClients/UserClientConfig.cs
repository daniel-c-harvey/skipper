using Web.Shared.ApiClients;

namespace AuthBlocksWeb.ApiClients;

public class UserClientConfig : ModelControllerClientConfig
{
    public UserClientConfig(string baseURL, int port) : base(baseURL, port, "users")
    {
    }

    public UserClientConfig(string url) : base(url, "users")
    {
    }
}