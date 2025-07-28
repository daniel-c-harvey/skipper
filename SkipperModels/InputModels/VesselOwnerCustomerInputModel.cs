using Models.Shared.InputModels;

namespace SkipperModels.InputModels
{
    public class VesselOwnerCustomerInputModel : InputModelBase
    {
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public ContactInputModel Contact { get; set; }
        public ICollection<VesselInputModel> Vessels { get; set; }
    }
} 