using Models.Shared.Models;

namespace SkipperModels.Models
{
    public class OrderModel<TCustomer> : BaseModel
    where TCustomer : CustomerModel
    {
        public string OrderNumber { get; set; }
        public TCustomer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderType OrderType { get; set; }
        public int TotalAmount { get; set; }
        public string? Notes { get; set; }
        public OrderStatus Status { get; set; }
    }
} 