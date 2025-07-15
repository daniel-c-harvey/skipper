using Web.Shared.ApiClients;

namespace SkipperWeb.ApiClients;

public class RentalAgreementClientConfig : ModelClientConfig
{
    public RentalAgreementClientConfig(string baseURL, int port) : base(baseURL, port, "rentalagreement") { }
    
    public RentalAgreementClientConfig(string url) : base(url, "rentalagreement") { }
}