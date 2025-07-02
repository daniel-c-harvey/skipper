using System.ComponentModel.DataAnnotations;
using SkipperModels.Common;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperModels.InputModels;

public class VesselInputModel : IInputModel<VesselInputModel, VesselModel, VesselEntity>
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
    [LengthRange]
    [Display(Name = "Length")]
    public decimal Length { get; set; }

    [Required(ErrorMessage = "Beam is required")]
    [BeamRange]
    [Display(Name = "Beam")]
    public decimal Beam { get; set; }

    [Required(ErrorMessage = "Vessel type is required")]
    [Display(Name = "Vessel Type")]
    public VesselType VesselType { get; set; }

    public static VesselModel MakeModel(VesselInputModel input)
    {
        return new VesselModel()
        {
            RegistrationNumber = input.RegistrationNumber,
            Name = input.Name,
            Length = input.Length,
            Beam = input.Beam,
            VesselType = input.VesselType
        };
    }
}