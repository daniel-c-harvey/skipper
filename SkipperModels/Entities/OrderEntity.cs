using Models.Shared.Entities;
using Models.Shared;

namespace SkipperModels.Entities;

public abstract class OrderEntity<TCustomer> : BaseEntity, IEntity
    where TCustomer : CustomerEntity
{
    public string OrderNumber { get; set; }
    public long CustomerId { get; set; }
    public virtual TCustomer Customer { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderType OrderType { get; set; } // TPH Discriminator
    public int TotalAmount { get; set; } // cents
    public string? Notes { get; set; }
    public OrderStatus Status { get; set; }
} 