using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters
{
    public class CustomerEntityToModelConverter : IEntityToModelConverter<CustomerEntity, CustomerModel>
    {
        public static CustomerModel Convert(CustomerEntity entity)
        {
            return new CustomerModel
            {
                Id = entity.Id,
                AccountNumber = entity.AccountNumber,
                Name = entity.Name,
                CustomerProfileType = entity.CustomerProfileType,
                CustomerProfileId = entity.CustomerProfileId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public static CustomerEntity Convert(CustomerModel model)
        {
            return new CustomerEntity
            {
                Id = model.Id,
                AccountNumber = model.AccountNumber,
                Name = model.Name,
                CustomerProfileType = model.CustomerProfileType,
                CustomerProfileId = model.CustomerProfileId,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
            };
        }
    }

    public class CustomerModelToInputConverter : IModelToInputConverter<CustomerModel, CustomerInputModel>
    {
        public static CustomerInputModel Convert(CustomerModel model)
        {
            return new CustomerInputModel
            {
                Id = model.Id,
                AccountNumber = model.AccountNumber,
                Name = model.Name,
                CustomerProfileType = model.CustomerProfileType,
                CustomerProfileId = model.CustomerProfileId,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
            };
        }

        public static CustomerModel Convert(CustomerInputModel input)
        {
            return new CustomerModel
            {
                Id = input.Id,
                AccountNumber = input.AccountNumber,
                Name = input.Name,
                CustomerProfileType = input.CustomerProfileType,
                CustomerProfileId = input.CustomerProfileId,
                CreatedAt = input.CreatedAt,
                UpdatedAt = input.UpdatedAt,
            };
        }
    }
}