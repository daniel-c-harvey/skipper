using NetBlocks.Utilities;

namespace SkipperModels.InputModels;

public class OrderStatusEnumeration : DisplayEnumeration<OrderStatusEnumeration>
{
    public static OrderStatusEnumeration Draft = new(OrderStatus.Draft, "Draft");
    public static OrderStatusEnumeration Quoted = new(OrderStatus.Quoted, "Quoted");
    public static OrderStatusEnumeration Pending = new(OrderStatus.Pending, "Pending");
    public static OrderStatusEnumeration Confirmed = new(OrderStatus.Confirmed, "Confirmed");
    public static OrderStatusEnumeration InProgress = new(OrderStatus.InProgress, "In Progress");
    public static OrderStatusEnumeration Completed = new(OrderStatus.Completed, "Completed");
    public static OrderStatusEnumeration Cancelled = new(OrderStatus.Cancelled, "Cancelled");
    public static OrderStatusEnumeration Expired = new(OrderStatus.Expired, "Expired");
    
    public OrderStatus OrderStatus { get; init; }
    
    private OrderStatusEnumeration(OrderStatus orderStatus, string displayName) : base((int)orderStatus, orderStatus.ToString("G"), displayName)
    {
        OrderStatus = orderStatus;
    }
} 