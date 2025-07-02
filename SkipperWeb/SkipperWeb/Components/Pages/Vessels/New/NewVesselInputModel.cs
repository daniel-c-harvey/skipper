using System.ComponentModel.DataAnnotations;
using SkipperModels;

namespace SkipperWeb.Components.Pages.Vessels.New;

public class NewVesselInputModel
{
    [Required(ErrorMessage = "Registration number is required")]
    [StringLength(50, ErrorMessage = "Registration number cannot exceed 50 characters")]
    [Display(Name = "Registration Number")]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vessel name is required")]
    [StringLength(255, ErrorMessage = "Vessel name cannot exceed 255 characters")]
    [Display(Name = "Vessel Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Length is required")]
    [Range(0.01, 999999.99999, ErrorMessage = "Length must be greater than 0 and less than 1,000,000")]
    [Display(Name = "Length")]
    public decimal Length { get; set; }

    [Required(ErrorMessage = "Beam is required")]
    [Range(0.01, 999999.99999, ErrorMessage = "Beam must be greater than 0 and less than 1,000,000")]
    [Display(Name = "Beam")]
    public decimal Beam { get; set; }

    [Required(ErrorMessage = "Vessel type is required")]
    [Display(Name = "Vessel Type")]
    public VesselType VesselType { get; set; }
}