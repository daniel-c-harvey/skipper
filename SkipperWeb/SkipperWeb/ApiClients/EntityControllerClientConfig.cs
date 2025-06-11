using NetBlocks.Models;

namespace SkipperWeb.ApiClients;

public class EntityControllerClientConfig : ClientConfig
{
    public string ControllerName { get; protected set; }
    
    public EntityControllerClientConfig(string baseURL, int port, string controllerName) : base(baseURL, port)
    {
        ControllerName = controllerName;
    }

    public EntityControllerClientConfig(string url, string controllerName) : base(url)
    {
        ControllerName = controllerName;
    }
}