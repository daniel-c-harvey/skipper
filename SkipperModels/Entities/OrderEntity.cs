using Models.Shared.Entities;

namespace SkipperModels.Entities;

public abstract class OrderEntity<TCustomerProfile> : BaseLinkageEntity, ICompositeEntityRoot<OrderType>
where TCustomerProfile : CustomerProfileBaseEntity
{
    public string OrderNumber { get; set; }
    public long CustomerId { get; set; }
    public virtual TCustomerProfile Customer { get; set; }
    public DateTime OrderDate { get; set; }
    
    // Polymorphic association to order details
    public OrderType OrderType { get; set; }
    public long OrderTypeId { get; set; }
    
    public int TotalAmount { get; set; } // cents
    public string? Notes { get; set; }
    public OrderStatus Status { get; set; }

    public long Id
    {
        get => OrderTypeId;
        set => OrderTypeId = value;
    }
    public OrderType Discriminator
    {
        get => OrderType; 
        set => OrderType = value; 
    }
} 