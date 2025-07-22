using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters
{
    public class AddressEntityToModelConverter : IEntityToModelConverter<AddressEntity, AddressModel>
    {
        public static AddressModel Convert(AddressEntity entity)
        {
            return new AddressModel
            {
                Id = entity.Id,
                Address1 = entity.Address1,
                Address2 = entity.Address2,
                City = entity.City,
                State = entity.State,
                ZipCode = entity.ZipCode,
                Country = entity.Country,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static AddressEntity Convert(AddressModel model)
        {
            return new AddressEntity
            {
                Id = model.Id,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                Country = model.Country,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }
    }

    public class AddressModelToInputConverter : IModelToInputConverter<AddressModel, AddressInputModel>
    {
        public static AddressInputModel Convert(AddressModel model)
        {
            return new AddressInputModel
            {
                Id = model.Id,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                Country = model.Country,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
            };
        }

        public static AddressModel Convert(AddressInputModel input)
        {
            return new AddressModel
            {
                Id = input.Id,
                Address1 = input.Address1,
                Address2 = input.Address2,
                City = input.City,
                State = input.State,
                ZipCode = input.ZipCode,
                Country = input.Country,
                CreatedAt = input.CreatedAt,
                UpdatedAt = input.UpdatedAt,
            };
        }
    }
}