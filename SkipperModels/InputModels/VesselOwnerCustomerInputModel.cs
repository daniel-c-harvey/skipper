using System.ComponentModel.DataAnnotations;
using Models.Shared.InputModels;

namespace SkipperModels.InputModels
{
    public class VesselOwnerCustomerInputModel : CustomerInputModel
    {
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        [Required] public ContactInputModel Contact { get; set; } = new();
        [Required] public ICollection<VesselInputModel> Vessels { get; set; } = new List<VesselInputModel>();
    }
} 