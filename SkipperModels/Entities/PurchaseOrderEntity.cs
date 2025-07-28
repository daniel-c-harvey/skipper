using Models.Shared.Entities;

namespace SkipperModels.Entities;

public class PurchaseOrderEntity : OrderEntity<BusinessCustomerEntity>
{
    // Purchase-specific properties
    public string PurchaseOrderNumber { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public string ShippingAddress { get; set; }
    public string BillingAddress { get; set; }
    public PurchaseOrderStatus PurchaseStatus { get; set; }
    public string TermsAndConditions { get; set; }

    public PurchaseOrderEntity()
    {
        OrderType = OrderType.PurchaseOrder;
    }
}

public enum PurchaseOrderStatus
{
    Draft,
    Submitted,
    Approved,
    Ordered,
    Shipped,
    Delivered,
    Cancelled
} 