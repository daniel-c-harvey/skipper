using Data.Shared.Managers;
using Models.Shared.Converters;
using Models.Shared.Entities;
using Models.Shared.Models;
using SkipperData.Data.Repositories;
using SkipperModels;
using SkipperModels.Composites;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperData.Managers;

public abstract class OrderManager<TCustomerProfile, TCompositeOrderEntity, TOrderEntity, TOrderInfoEntity, TCompositeOrderModel, TOrderInfoModel, TRepository, TCompositeConverter, TInfoConverter> 
    : CompositeManager<TCompositeOrderEntity, TOrderEntity, TOrderInfoEntity, 
                       TCompositeOrderModel, OrderModel, TOrderInfoModel,
                       OrderType, TRepository, TCompositeConverter>
    where TCustomerProfile : CustomerProfileBaseEntity
    where TCompositeOrderEntity : class, IOrderCompositeEntity<TCustomerProfile, TOrderEntity, TOrderInfoEntity>, new()
    where TOrderEntity : OrderEntity<TCustomerProfile>, new()
    where TOrderInfoEntity : class, IEntity, new()
    where TCompositeOrderModel : class, IOrderCompositeModel<TOrderInfoModel>, new()
    where TOrderInfoModel : class, IModel, new()
    where TRepository : IOrderRepository<TCustomerProfile, TCompositeOrderEntity, TOrderEntity, TOrderInfoEntity>
    where TCompositeConverter : OrderEntityToModelConverter<TCustomerProfile, TCompositeOrderEntity, TOrderEntity, TOrderInfoEntity, TCompositeOrderModel, TOrderInfoModel, TInfoConverter>
    where TInfoConverter : IEntityToModelConverter<TOrderInfoEntity, TOrderInfoModel>
{
    protected OrderManager(TRepository repository) : base(repository)
    {
    }
}