using Models.Shared.Converters;
using Models.Shared.Entities;
using Models.Shared.Models;
using SkipperModels.Composites;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters
{
    public class OrderEntityToModelConverter<TCustomerProfile, TOrderEntity> : IEntityToModelConverter<TOrderEntity, OrderModel>
        where TCustomerProfile : CustomerProfileBaseEntity
        where TOrderEntity : OrderEntity<TCustomerProfile>, new()
    {
        public static OrderModel Convert(TOrderEntity entity)
        {
            return new OrderModel
            {
                Id = entity.Id,
                OrderNumber = entity.OrderNumber,
                CustomerId = entity.CustomerId,
                OrderDate = entity.OrderDate,
                OrderType = entity.OrderType,
                OrderTypeId = entity.OrderTypeId,
                TotalAmount = entity.TotalAmount,
                Notes = entity.Notes,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public static TOrderEntity Convert(OrderModel model)
        {
            return new TOrderEntity
            {
                Id = model.Id,
                OrderNumber = model.OrderNumber,
                CustomerId = model.CustomerId,
                OrderDate = model.OrderDate,
                OrderType = model.OrderType,
                OrderTypeId = model.OrderTypeId,
                TotalAmount = model.TotalAmount,
                Notes = model.Notes,
                Status = model.Status,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
            };
        }
    }
    
    public class OrderEntityToModelConverter<TCustomerProfile, TOrderCompositeEntity, TOrderEntity, TOrderInfoEntity, TOrderCompositeModel, TOrderInfoModel, TInfoConverter>
        : ICompositeEntityToModelConverter<TOrderCompositeEntity, TOrderEntity, TOrderInfoEntity, TOrderCompositeModel, OrderModel, TOrderInfoModel, OrderType>
        where TCustomerProfile : CustomerProfileBaseEntity
        where TOrderCompositeEntity : class, IOrderCompositeEntity<TCustomerProfile, TOrderEntity, TOrderInfoEntity>, new()
        where TOrderEntity : OrderEntity<TCustomerProfile>, new()
        where TOrderInfoEntity : class, IEntity, new()
        where TOrderCompositeModel : class, IOrderCompositeModel<TOrderInfoModel>, new()
        where TOrderInfoModel : class, IModel, new()
        where TInfoConverter : IEntityToModelConverter<TOrderInfoEntity, TOrderInfoModel>
    
    {
        public static TOrderCompositeModel Convert(TOrderCompositeEntity entity)
        {
            return new TOrderCompositeModel
            {
                Id = entity.Id,
                Root = OrderEntityToModelConverter<TCustomerProfile, TOrderEntity>.Convert(entity.Root),
                Info = TInfoConverter.Convert(entity.Info),
            };
        }

        public static TOrderCompositeEntity Convert(TOrderCompositeModel model)
        {
            return new TOrderCompositeEntity
            {
                Id = model.Id,
                Root = OrderEntityToModelConverter<TCustomerProfile, TOrderEntity>.Convert(model.Root),
                Info = TInfoConverter.Convert(model.Info),
            };
        }
    }

    public class OrderModelToInputConverter : IModelToInputConverter<OrderModel, OrderInputModel>
    {
        public static OrderInputModel Convert(OrderModel model)
        {
            return new OrderInputModel
            {
                Id = model.Id,
                OrderNumber = model.OrderNumber,
                CustomerId = model.CustomerId,
                OrderDate = model.OrderDate,
                OrderType = model.OrderType,
                OrderTypeId = model.OrderTypeId,
                TotalAmount = (decimal)model.TotalAmount / 100M, // convert to dollars 
                Notes = model.Notes,
                Status = model.Status,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
            };
        }

        public static OrderModel Convert(OrderInputModel input)
        {
            return new OrderModel
            {
                Id = input.Id,
                OrderNumber = input.OrderNumber,
                CustomerId = input.CustomerId,
                OrderDate = input.OrderDate,
                OrderType = input.OrderType,
                OrderTypeId = input.OrderTypeId,
                TotalAmount = (int)(Math.Round(input.TotalAmount, 2) * 100),
                Notes = input.Notes,
                Status = input.Status,
                CreatedAt = input.CreatedAt,
                UpdatedAt = input.UpdatedAt,
            };
        }
    }
} 