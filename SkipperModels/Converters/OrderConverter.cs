using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters
{
    // Generic base converter for OrderEntity and its derived types
    public class OrderConverter<TOrderEntity, TOrderModel, TCustomerEntity, TCustomerModel> : IEntityToModelConverter<TOrderEntity, TOrderModel>
        where TOrderEntity : OrderEntity, new()
        where TOrderModel : OrderModel<TCustomerModel>, new()
        where TCustomerEntity : CustomerEntity, new()
        where TCustomerModel : CustomerModel, new()
    {
        public static TOrderModel Convert(TOrderEntity entity)
        {
            var model = new TOrderModel
            {
                Id = entity.Id,
                OrderNumber = entity.OrderNumber,
                Customer = CustomerEntityToModelConverter<TCustomerEntity, TCustomerModel>.ConvertBase(entity.Customer),
                OrderDate = entity.OrderDate,
                OrderType = entity.OrderType,
                TotalAmount = entity.TotalAmount,
                Notes = entity.Notes,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
            return model;
        }

        public static TOrderEntity Convert(TOrderModel model)
        {
            var entity = new TOrderEntity
            {
                Id = model.Id,
                OrderNumber = model.OrderNumber,
                CustomerId = model.Customer.Id,
                OrderDate = model.OrderDate,
                OrderType = model.OrderType,
                TotalAmount = model.TotalAmount,
                Notes = model.Notes,
                Status = model.Status,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
            };
            return entity;
        }
    }
} 