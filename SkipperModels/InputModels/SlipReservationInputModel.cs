using System.ComponentModel.DataAnnotations;
using Models.Shared.InputModels;

namespace SkipperModels.InputModels;

public class SlipReservationInputModel : IInputModel
{
    public long Id { get; set; }
    
    [Required]
    public SlipInputModel? Slip { get; set; }
    [Required]
    public VesselInputModel? Vessel { get; set; }
    [Required(ErrorMessage = "Start Date is required")]
    public DateTime? StartDate { get; set; }
    [Required(ErrorMessage = "End Date is required")]
    public DateTime? EndDate { get; set; }
    [Required(ErrorMessage = "Price is required")]
    public decimal PriceRate { get; set; }
    [Required(ErrorMessage = "Unit is required")]
    public PriceUnitEnumeration? PriceUnit { get; set; }
    [Required]
    public RentalStatusEnumeration? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
} 