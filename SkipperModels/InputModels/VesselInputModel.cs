using System.ComponentModel.DataAnnotations;
using Models.Shared.InputModels;
using SkipperModels.Common;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperModels.InputModels;

public class VesselInputModel : IInputModel<VesselInputModel, VesselModel, VesselEntity>, IEquatable<VesselInputModel>
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

    public static VesselModel MakeModel(VesselInputModel input)
    {
        return new VesselModel()
        {
            Id = input.Id,
            RegistrationNumber = input.RegistrationNumber,
            Name = input.Name,
            Length = input.Length,
            Beam = input.Beam,
            VesselType = input.VesselType,
            CreatedAt = input.CreatedAt,
            UpdatedAt = input.UpdatedAt
        };
    }

    public static VesselInputModel From(VesselModel model)
    {
        return new VesselInputModel()
        {
            Id = model.Id,
            RegistrationNumber = model.RegistrationNumber,
            Name = model.Name,
            Length = model.Length,
            Beam = model.Beam,
            VesselType = model.VesselType,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }

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