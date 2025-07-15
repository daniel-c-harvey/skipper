using System.ComponentModel.DataAnnotations;
using Models.Shared.InputModels;

namespace SkipperModels.InputModels;

public class SlipInputModel : IInputModel, IEquatable<SlipInputModel>
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Slip Number is required")]
    [Display(Name = "Slip Number")]
    public string SlipNumber { get; set; }
    
    [Required(ErrorMessage = "Slip Classification is required")]
    [Display(Name = "Slip Classification")]
    public SlipClassificationInputModel SlipClassification { get; set; }
    
    [Required(ErrorMessage = "Location Code is required")]
    [Display(Name = "Location Code")]
    public string LocationCode { get; set; }
    
    [Required(ErrorMessage = "Slip Status is required")]
    [Display(Name = "Slip Status")]
    public SlipStatusEnumeration Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public bool Equals(SlipInputModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && 
               SlipNumber == other.SlipNumber && 
               SlipClassification.Equals(other.SlipClassification) 
               && LocationCode == other.LocationCode 
               && Status == other.Status;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((SlipInputModel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, SlipNumber, SlipClassification, LocationCode, (int)Status);
    }

    public override string ToString()
    {
        return SlipNumber;
    }
}