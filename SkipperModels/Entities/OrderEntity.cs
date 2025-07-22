using Models.Shared.Entities;

namespace SkipperModels.Entities;

public class OrderEntity : BaseEntity, IEntity
{
    public string OrderNumber { get; set; }
    public long CustomerId { get; set; }
    public virtual CustomerEntity Customer { get; set; }
    public DateTime OrderDate { get; set; }
    
    // Polymorphic association to order details
    public OrderType OrderType { get; set; }
    public long OrderTypeId { get; set; }
    
    public int TotalAmount { get; set; } // cents
    public string? Notes { get; set; }
    public OrderStatus Status { get; set; }
} 