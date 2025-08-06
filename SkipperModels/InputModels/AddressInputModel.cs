using System.ComponentModel.DataAnnotations;
using Models.Shared.InputModels;

namespace SkipperModels.InputModels
{
    public class AddressInputModel : InputModelBase
    {
        [Required]
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string Country { get; set; }
    }
}