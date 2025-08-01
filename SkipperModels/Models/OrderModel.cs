using Models.Shared.Models;

namespace SkipperModels.Models
{
    public class OrderModel<TCustomerModel> : BaseModel
    where TCustomerModel : CustomerModel
    {
        public string OrderNumber { get; set; }
        public TCustomerModel Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderType OrderType { get; set; }
        public int TotalAmount { get; set; }
        public string? Notes { get; set; }
        public OrderStatus Status { get; set; }
    }
    
    public class OrderModel : OrderModel<CustomerModel>
    {
    }
} 