using Models.Shared.Models;

namespace SkipperModels.Models
{
    public class VesselOwnerCustomerModel : CustomerModel
    {
        // Vessel owner specific properties
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        
        // Contact relationship
        public ContactModel Contact { get; set; }
        
        // Vessel relationships
        public ICollection<VesselModel> Vessels { get; set; }
    }
} 