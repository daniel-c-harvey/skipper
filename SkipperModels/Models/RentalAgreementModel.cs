using SkipperModels.Entities;

namespace SkipperModels.Models;


public class RentalAgreementModel : BaseModel<RentalAgreementModel, RentalAgreementEntity>, IModel<RentalAgreementModel, RentalAgreementEntity>
{
    public SlipModel Slip { get; set; }
    public VesselModel Vessel { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PriceRate { get; set; }
    public PriceUnit PriceUnit { get; set; }
    public RentalStatus Status { get; set; }
    public static RentalAgreementEntity CreateEntity(RentalAgreementModel model)
    {
        return new RentalAgreementEntity()
        {
            Id = model.Id,
            SlipId = model.Slip.Id,
            SlipEntity = SlipModel.CreateEntity(model.Slip),
            VesselId = model.Vessel.Id,
            VesselEntity = VesselModel.CreateEntity(model.Vessel),
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            PriceRate = model.PriceRate,
            PriceUnit = model.PriceUnit,
            Status = model.Status,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }
}
