using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients;

public class SlipReservationClientConfig : ModelClientConfig
{
    public SlipReservationClientConfig(string baseURL, int port) : base(baseURL, port, "slipreservation") { }
    
    public SlipReservationClientConfig(string url) : base(url, "slipreservation") { }
} 