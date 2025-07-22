using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters
{
    public class OrderEntityToModelConverter : IEntityToModelConverter<OrderEntity, OrderModel>
    {
        public static OrderModel Convert(OrderEntity entity)
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

        public static OrderEntity Convert(OrderModel model)
        {
            return new OrderEntity
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