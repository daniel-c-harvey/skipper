using System.Linq.Expressions;
using Data.Shared.Data.Repositories;
using Microsoft.Extensions.Logging;
using Models.Shared.Common;
using Models.Shared.Entities;
using NetBlocks.Models;
using SkipperModels;
using SkipperModels.Composites;
using SkipperModels.Entities;

namespace SkipperData.Data.Repositories;

public class OrderRepository<TCustomerProfile, TOrderCompositeEntity, TOrderEntity, TOrderInfoEntity> 
    : CompositeRepository<SkipperContext, TOrderCompositeEntity, TOrderEntity, OrderType, TOrderInfoEntity>, 
        IOrderRepository<TCustomerProfile, TOrderCompositeEntity, TOrderEntity, TOrderInfoEntity>
    where TCustomerProfile : CustomerProfileBaseEntity
    where TOrderEntity : OrderEntity<TCustomerProfile>, new()
    where TOrderCompositeEntity : class, IOrderCompositeEntity<TCustomerProfile, TOrderEntity, TOrderInfoEntity>, new()
    where TOrderInfoEntity : class, IEntity
{
    public OrderRepository(SkipperContext context, 
                           OrderType discriminator, 
                           ILogger<OrderRepository<TCustomerProfile, TOrderCompositeEntity, TOrderEntity, TOrderInfoEntity>> logger)
        : base(context, discriminator, logger)
    {
    }
}