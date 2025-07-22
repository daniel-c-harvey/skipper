using Models.Shared.InputModels;

namespace SkipperModels.InputModels
{
    public class OrderInputModel : InputModelBase
    {
        public string OrderNumber { get; set; }
        public long CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderType OrderType { get; set; }
        public long OrderTypeId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public OrderStatus Status { get; set; }
    }
} 