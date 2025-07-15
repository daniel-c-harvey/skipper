using System.ComponentModel.DataAnnotations;
using Models.Shared.InputModels;
using SkipperModels.Common;
namespace SkipperModels.InputModels;

public class VesselInputModel : IInputModel, IEquatable<VesselInputModel>
{
    public long Id { get; set; }
    
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
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool Equals(VesselInputModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id &&
               RegistrationNumber == other.RegistrationNumber && 
               Name == other.Name && 
               Length == other.Length &&
               Beam == other.Beam &&
               VesselType == other.VesselType;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((VesselInputModel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, RegistrationNumber, Name, Length, Beam, (int)VesselType);
    }

    public override string ToString()
    {
        return Name;
    }
}