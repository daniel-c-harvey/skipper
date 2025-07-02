using System.ComponentModel.DataAnnotations;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperModels.InputModels;

public class SlipInputModel : IInputModel<SlipInputModel, SlipModel, SlipEntity>
{
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
    public SlipStatus Status { get; set; }
    
    public static SlipModel MakeModel(SlipInputModel input)
    {
        return new SlipModel()
        {
            SlipClassification = SlipClassificationInputModel.MakeModel(input.SlipClassification),
            SlipNumber = input.SlipNumber,
            LocationCode = input.LocationCode,
            Status = input.Status
        };
    }
}