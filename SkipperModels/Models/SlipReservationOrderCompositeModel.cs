using SkipperModels.Composites;

namespace SkipperModels.Models;

public class SlipReservationOrderCompositeModel : IOrderCompositeModel<SlipReservationModel>
{
    public OrderModel Order { get; set; }
    public SlipReservationModel OrderInfo { get; set; }

    public long Id
    {
        get => Order.Id;
        set
        {
            Order.Id = value;
            OrderInfo.Id = value;
        }
    }

    public OrderModel Root
    {
        get => Order;
        set => Order = value;
    }

    public SlipReservationModel Info
    {
        get => OrderInfo; 
        set => OrderInfo = value;
    }
}