using Data.Shared.Data.Repositories;
using Models.Shared.Entities;
using SkipperModels;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public interface IOrderRepository<TCustomerProfile, TOrderCompositeEntity, TOrderEntity, TOrderInfoEntity> 
    : ICompositeRepository<TOrderCompositeEntity, TOrderEntity, OrderType, TOrderInfoEntity>
    where TCustomerProfile : CustomerProfileBaseEntity
    where TOrderCompositeEntity : class, ICompositeEntity<TOrderEntity, OrderType, TOrderInfoEntity>
    where TOrderEntity : OrderEntity<TCustomerProfile>, new()
    where TOrderInfoEntity : IEntity 
{
}