using API.Shared.Controllers;
using Models.Shared.Converters;
using Models.Shared.Entities;
using Models.Shared.Models;
using SkipperData.Data.Repositories;
using SkipperData.Managers;
using SkipperModels;
using SkipperModels.Composites;
using SkipperModels.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperAPI.Controllers;


public abstract class OrderController<
    TCustomerProfile,
    TOrderCompositeEntity, 
    TOrderEntity,
    TOrderInfoEntity, 
    TOrderCompositeModel, 
    TOrderInfoModel,
    TOrderManager,
    TOrderRepository,
    TOrderCompositeConverter,
    TOrderInfoConverter>  
: CompositeController<TOrderCompositeEntity, 
    TOrderEntity, 
    TOrderInfoEntity, 
    TOrderCompositeModel, 
    OrderModel, 
    TOrderInfoModel, 
    OrderType, 
    TOrderManager>
where TCustomerProfile : CustomerProfileBaseEntity, new()
where TOrderCompositeEntity : class, IOrderCompositeEntity<TCustomerProfile, TOrderEntity, TOrderInfoEntity>, new()
where TOrderEntity : OrderEntity<TCustomerProfile>, new()
where TOrderInfoEntity : class, IEntity, new()
where TOrderCompositeModel : class, IOrderCompositeModel<TOrderInfoModel>, new()
where TOrderInfoModel : class, IModel, new()
where TOrderManager : OrderManager<
                        TCustomerProfile,
                        TOrderCompositeEntity,
                        TOrderEntity,
                        TOrderInfoEntity,
                        TOrderCompositeModel,
                        TOrderInfoModel, 
                        TOrderRepository, 
                        TOrderCompositeConverter,
                        TOrderInfoConverter>
where TOrderRepository : OrderRepository<TCustomerProfile, TOrderCompositeEntity, TOrderEntity, TOrderInfoEntity>
where TOrderInfoConverter : class, IEntityToModelConverter<TOrderInfoEntity, TOrderInfoModel>
where TOrderCompositeConverter : OrderEntityToModelConverter<
                                    TCustomerProfile,
                                    TOrderCompositeEntity,
                                    TOrderEntity,
                                    TOrderInfoEntity,
                                    TOrderCompositeModel,
                                    TOrderInfoModel,
                                    TOrderInfoConverter>
{
    public OrderController(TOrderManager manager) 
    : base(manager)
    {
    }
}