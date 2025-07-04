using SkipperModels.Models;

namespace SkipperModels.Entities;

public class RentalAgreementEntity : BaseEntity<RentalAgreementEntity, RentalAgreementModel>, IEntity<RentalAgreementEntity, RentalAgreementModel>
{
    public long SlipId { get; set; }
    public virtual SlipEntity SlipEntity { get; set; }
    public long VesselId { get; set; }
    public virtual VesselEntity VesselEntity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PriceRate { get; set; }
    public PriceUnit PriceUnit { get; set; }
    public RentalStatus Status { get; set; }
    public static RentalAgreementModel CreateModel(RentalAgreementEntity entity)
    {
        return new RentalAgreementModel()
        {
            Id = entity.Id,
            Slip = SlipEntity.CreateModel(entity.SlipEntity),
            Vessel = VesselEntity.CreateModel(entity.VesselEntity),
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            PriceRate = entity.PriceRate / 100M,
            PriceUnit = entity.PriceUnit,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }
}
