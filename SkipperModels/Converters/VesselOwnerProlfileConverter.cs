using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters
{
    public class VesselOwnerProfileEntityToModelConverter : IEntityToModelConverter<VesselOwnerProfileEntity, VesselOwnerProfileModel>
    {
        public static VesselOwnerProfileModel Convert(VesselOwnerProfileEntity entity)
        {
            return new VesselOwnerProfileModel
            {
                Id = entity.Id,
                Contact = ContactEntityToModelConverter.Convert(entity.Contact),
                Vessels = entity.VesselOwnerVessels.Select(e => VesselEntityToModelConverter.Convert(e.Vessel)).ToList() ?? new List<VesselModel>(),
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public static VesselOwnerProfileEntity Convert(VesselOwnerProfileModel model)
        {
            return new VesselOwnerProfileEntity
            {
                Id = model.Id,
                ContactId = model.Contact.Id,
                VesselOwnerVessels = model.Vessels.Select(e => new VesselOwnerVesselEntity
                {
                    VesselId = e.Id,
                    VesselOwnerProfileId = model.Id,
                }).ToList(),
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
            };
        }
    }
    
    public class VesselOwnerProfileModelToInputConverter : IModelToInputConverter<VesselOwnerProfileModel, VesselOwnerProfileInputModel>
    {
        public static VesselOwnerProfileInputModel Convert(VesselOwnerProfileModel model)
        {
            return new VesselOwnerProfileInputModel
            {
                Id = model.Id,
                Contact = ContactModelToInputConverter.Convert(model.Contact),
                Vessels = model.Vessels.Select(e => VesselModelToInputConverter.Convert(e)).ToList(),
            };
        }

        public static VesselOwnerProfileModel Convert(VesselOwnerProfileInputModel input)
        {
            return new VesselOwnerProfileModel
            {
                Id = input.Id,
                Contact = ContactModelToInputConverter.Convert(input.Contact),
                Vessels = input.Vessels.Select(e => VesselModelToInputConverter.Convert(e)).ToList(),
            };
        }
    }
}