using Models.Shared.Entities;
using SkipperModels.Entities;

namespace SkipperModels.Composites;

public interface IOrderCompositeEntity<TCustomerProfile, TOrder, TInfo> : ICompositeEntity<TOrder, OrderType, TInfo>
    where TCustomerProfile : CustomerProfileBaseEntity
    where TOrder : OrderEntity<TCustomerProfile>
    where TInfo : IEntity
{
    TOrder Order { get; set; }
    TInfo OrderInfo { get; set; }
}