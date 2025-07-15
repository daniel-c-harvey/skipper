using SkipperModels.Common;
using SkipperModels.Models;
using System.ComponentModel.DataAnnotations;
using Models.Shared.InputModels;

namespace SkipperModels.InputModels;

public class SlipClassificationInputModel 
: IInputModel,
  IEquatable<SlipClassificationInputModel>
{
    public long Id { get; set; }
    [Required(ErrorMessage = "Name required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Description required")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Base Price required")]
    public decimal BasePrice { get; set; }
    [Required(ErrorMessage = "Max Length required")]
    [LengthRange]
    public decimal MaxLength { get; set; }
    [Required(ErrorMessage = "Max Beam required")]
    [BeamRange]
    public decimal MaxBeam { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    

    public static SlipClassificationInputModel From(SlipClassificationModel model)
    {
        return new SlipClassificationInputModel()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            BasePrice = model.BasePrice / 100M,
            MaxLength = model.MaxLength,
            MaxBeam = model.MaxBeam,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }

    public bool Equals(SlipClassificationInputModel? other)
    {
        if (other is null) return false;
        return Id == other.Id &&
               Name == other.Name &&
               Description == other.Description &&
               BasePrice == other.BasePrice &&
               MaxBeam == other.MaxBeam &&
               MaxLength == other.MaxLength;
    }

    public override string ToString()
    {
        return $"{Name}";
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as SlipClassificationInputModel);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Description, BasePrice, MaxLength, MaxBeam);
    }
}