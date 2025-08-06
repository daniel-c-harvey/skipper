using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters
{
    public class CustomerEntityToModelConverter<TCustomerEntity, TCustomerModel> : IEntityToModelConverter<TCustomerEntity, TCustomerModel>
        where TCustomerEntity : CustomerEntity, new()
        where TCustomerModel : CustomerModel, new()
    {
        public static TCustomerModel ConvertBase(CustomerEntity entity)
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
        public static TCustomerModel Convert(TCustomerEntity entity)
        {
            return ConvertBase(entity);
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
    
    public class CustomerModelToInputConverter<TCustomerModel, TCustomerInput> : IModelToInputConverter<TCustomerModel, TCustomerInput>
        where TCustomerModel : CustomerModel, new()
        where TCustomerInput : CustomerInputModel, new()
    {
        public static TCustomerModel Convert(TCustomerInput input)
        {
            return new TCustomerModel
            {
                Id = input.Id,
                AccountNumber = input.AccountNumber,
                Name = input.Name,
                CustomerProfileType = input.CustomerProfileType,
                CreatedAt = input.CreatedAt,
                UpdatedAt = input.UpdatedAt
            };
        }

        public static TCustomerInput Convert(TCustomerModel model)
        {
            return new TCustomerInput
            {
                Id = model.Id,
                AccountNumber = model.AccountNumber,
                Name = model.Name,
                CustomerProfileType = model.CustomerProfileType,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }
    }
}