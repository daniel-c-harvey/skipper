using Models.Shared.Models;

namespace SkipperModels.Models
{
    public class OrderModel : BaseModel, IModel
    {
        public string OrderNumber { get; set; }
        public long CustomerId { get; set; }
        public CustomerModel? Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderType OrderType { get; set; }
        public long OrderTypeId { get; set; }
        public int TotalAmount { get; set; }
        public string? Notes { get; set; }
        public OrderStatus Status { get; set; }
    }
} 