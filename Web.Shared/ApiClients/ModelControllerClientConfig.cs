using NetBlocks.Models;

namespace Web.Shared.ApiClients;

public class ModelControllerClientConfig : ClientConfig
{
    public string ControllerName { get; protected set; }
    
    public ModelControllerClientConfig(string baseURL, int port, string controllerName) : base(baseURL, port)
    {
        ControllerName = controllerName;
    }

    public ModelControllerClientConfig(string url, string controllerName) : base(url)
    {
        ControllerName = controllerName;
    }
}