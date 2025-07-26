using SkipperModels.Composites;

namespace SkipperModels.Entities;

public class SlipReservationOrderCompositeEntity : IOrderCompositeEntity<VesselOwnerProfileEntity, VesselOwnerOrderEntity, SlipReservationEntity>
{
    public VesselOwnerOrderEntity Order { get; set; }
    public SlipReservationEntity OrderInfo { get; set; }

    public long Id
    {
        get => Order.Id;
        set
        {
            Order.Id = value;
            OrderInfo.Id = value;
        }
    }

    public VesselOwnerOrderEntity Root
    {
        get => Order;
        set => Order = value;
    }

    public SlipReservationEntity Info
    {
        get => OrderInfo; 
        set => OrderInfo = value;
    }
}