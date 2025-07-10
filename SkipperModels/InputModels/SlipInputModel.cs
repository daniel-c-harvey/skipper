using System.ComponentModel.DataAnnotations;
using Models.Shared.InputModels;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperModels.InputModels;

public class SlipInputModel : IInputModel<SlipInputModel, SlipModel, SlipEntity>, IEquatable<SlipInputModel>
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
    public static SlipModel MakeModel(SlipInputModel input)
    {
        return new SlipModel()
        {
            Id = input.Id,
            SlipClassification = SlipClassificationInputModel.MakeModel(input.SlipClassification),
            SlipNumber = input.SlipNumber,
            LocationCode = input.LocationCode,
            Status = input.Status.SlipStatus,
            CreatedAt = input.CreatedAt,
            UpdatedAt = input.UpdatedAt
        };
    }

    public static SlipInputModel From(SlipModel model)
    {
        return new SlipInputModel()
        {
            Id = model.Id,
            SlipNumber = model.SlipNumber,
            SlipClassification = SlipClassificationInputModel.From(model.SlipClassification),
            LocationCode = model.LocationCode,
            Status = SlipStatusEnumeration.GetById((int)model.Status)!,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
            
        };
    }

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