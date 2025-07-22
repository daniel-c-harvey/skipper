namespace SkipperModels
{
    /// <summary>
    /// Defines the type of order for polymorphic order relationships
    /// </summary>
    public enum OrderType
    {
        RentalAgreement,         // Links to RentalAgreementEntity
        ServiceOrder,            // Links to ServiceOrderEntity (future)
        PurchaseOrder,           // Links to PurchaseOrderEntity (future)
        StorageOrder             // Links to StorageOrderEntity (future)
    }
} 