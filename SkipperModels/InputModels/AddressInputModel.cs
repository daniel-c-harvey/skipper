using Models.Shared.InputModels;

namespace SkipperModels.InputModels
{
    public class AddressInputModel : InputModelBase
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}