using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters
{
    // Generic base converter for CustomerEntity and its derived types
    public class CustomerConverter<TCustomerEntity, TCustomerModel> : IEntityToModelConverter<TCustomerEntity, TCustomerModel>
        where TCustomerEntity : CustomerEntity, new()
        where TCustomerModel : CustomerModel, new()
    {
        public static TCustomerModel Convert(TCustomerEntity entity)
        {
            var model = new TCustomerModel
            {
                Id = entity.Id,
                AccountNumber = entity.AccountNumber,
                Name = entity.Name,
                CustomerProfileType = entity.CustomerProfileType,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
            return model;
        }

        public static TCustomerEntity Convert(TCustomerModel model)
        {
            var entity = new TCustomerEntity
            {
                Id = model.Id,
                AccountNumber = model.AccountNumber,
                Name = model.Name,
                CustomerProfileType = model.CustomerProfileType,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
            };
            return entity;
        }
    }
}