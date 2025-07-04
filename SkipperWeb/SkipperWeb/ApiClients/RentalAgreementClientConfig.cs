namespace SkipperWeb.ApiClients;

public class RentalAgreementClientConfig : ModelControllerClientConfig
{
    public RentalAgreementClientConfig(string baseURL, int port) : base(baseURL, port, "rentalagreement") { }
    
    public RentalAgreementClientConfig(string url) : base(url, "rentalagreement") { }
}