using Models.Shared.Entities;

namespace SkipperModels.Entities;

public class SlipReservationOrderEntity : OrderEntity
{
    // Slip-specific properties
    public long SlipId { get; set; }
    public virtual SlipEntity SlipEntity { get; set; }
    public long VesselId { get; set; }
    public virtual VesselEntity VesselEntity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PriceRate { get; set; }
    public PriceUnit PriceUnit { get; set; }
    public RentalStatus RentalStatus { get; set; }

    public SlipReservationOrderEntity()
    {
        OrderType = OrderType.SlipReservation;
    }
} 