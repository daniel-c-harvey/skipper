using System.ComponentModel.DataAnnotations;
using Models.Shared.InputModels;
using SkipperModels;

namespace SkipperModels.InputModels
{
    public class CustomerInputModel : InputModelBase
    {
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string Name { get; set; }
        public CustomerProfileType CustomerProfileType { get; set; }
    }
} 