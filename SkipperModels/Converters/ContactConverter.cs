using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters
{
    public class ContactEntityToModelConverter : IEntityToModelConverter<ContactEntity, ContactModel>
    {
        public static ContactModel Convert(ContactEntity entity)
        {
            return new ContactModel
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Address = AddressEntityToModelConverter.Convert(entity.Address),
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public static ContactEntity Convert(ContactModel model)
        {
            return new ContactEntity
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = AddressEntityToModelConverter.Convert(model.Address),
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
            };
        }
    }

    public class ContactModelToInputConverter : IModelToInputConverter<ContactModel, ContactInputModel>
    {
        public static ContactInputModel Convert(ContactModel model)
        {
            return new ContactInputModel
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = AddressModelToInputConverter.Convert(model.Address),
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
            };
        }

        public static ContactModel Convert(ContactInputModel input)
        {
            return new ContactModel
            {
                Id = input.Id,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                PhoneNumber = input.PhoneNumber,
                Address = AddressModelToInputConverter.Convert(input.Address),
                CreatedAt = input.CreatedAt,
                UpdatedAt = input.UpdatedAt,
            };
        }
    }
}