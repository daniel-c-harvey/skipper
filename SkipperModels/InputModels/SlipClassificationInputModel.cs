using SkipperModels.Common;
using SkipperModels.Entities;
using SkipperModels.Models;
using System.ComponentModel.DataAnnotations;

namespace SkipperModels.InputModels;

public class SlipClassificationInputModel 
: IInputModel<SlipClassificationInputModel, SlipClassificationModel, SlipClassificationEntity>,
  IEquatable<SlipClassificationInputModel>
{
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
    
    public static SlipClassificationModel MakeModel(SlipClassificationInputModel input)
    {
        return new SlipClassificationModel()
        {
            Name = input.Name,
            Description = input.Description,
            BasePrice = (int)Math.Round(input.BasePrice * 100, 0), // convert from dollars for front end to cents for data transfer.
            MaxLength = input.MaxLength,
            MaxBeam = input.MaxBeam,
        };
    }

    public bool Equals(SlipClassificationInputModel? other)
    {
        if (other is null) return false;
        return Name == other.Name &&
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
        return HashCode.Combine(Name, Description, BasePrice, MaxLength, MaxBeam);
    }
}