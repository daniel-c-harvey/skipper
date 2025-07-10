using System.ComponentModel.DataAnnotations;
using Models.Shared.InputModels;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperModels.InputModels;

public class RentalAgreementInputModel : IInputModel<RentalAgreementInputModel, RentalAgreementModel, RentalAgreementEntity>
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
    
    public static RentalAgreementModel MakeModel(RentalAgreementInputModel input)
    {
        return new RentalAgreementModel()
        {
            Id = input.Id,
            Slip = SlipInputModel.MakeModel(input.Slip),
            Vessel = VesselInputModel.MakeModel(input.Vessel),
            StartDate = input.StartDate ?? default,
            EndDate = input.EndDate ?? default,
            PriceRate = input.PriceRate,
            PriceUnit = input.PriceUnit.PriceUnit,
            Status = input.Status.RentalStatus,
            CreatedAt = input.CreatedAt,
            UpdatedAt = input.UpdatedAt
        };
    }

    public static RentalAgreementInputModel From(RentalAgreementModel model)
    {
        return new RentalAgreementInputModel()
        {
            Id = model.Id,
            Slip = SlipInputModel.From(model.Slip),
            Vessel = VesselInputModel.From(model.Vessel),
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            PriceRate = model.PriceRate,
            PriceUnit = PriceUnitEnumeration.GetById((int)model.PriceUnit)!,
            Status = RentalStatusEnumeration.GetById((int)model.Status)!,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }
}