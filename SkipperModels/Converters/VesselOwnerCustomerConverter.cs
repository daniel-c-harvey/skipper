using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters
{
    public class VesselOwnerCustomerConverter : IEntityToModelConverter<VesselOwnerCustomerEntity, VesselOwnerCustomerModel>
    {
        public static VesselOwnerCustomerModel Convert(VesselOwnerCustomerEntity entity)
        {
            // Use the generic base converter to set base properties, then add specific properties
            var model = CustomerConverter<VesselOwnerCustomerEntity, VesselOwnerCustomerModel>.Convert(entity);
            
            // Set VesselOwner-specific properties
            model.LicenseNumber = entity.LicenseNumber;
            model.LicenseExpiryDate = entity.LicenseExpiryDate;
            
            model.Contact = ContactEntityToModelConverter.Convert(entity.Contact);
            model.Vessels = entity.VesselOwnerVessels.Select(v => VesselEntityToModelConverter.Convert(v.Vessel)).ToList();
            
            return model;
        }

        public static VesselOwnerCustomerEntity Convert(VesselOwnerCustomerModel model)
        {
            // Use the generic base converter to set base properties, then add specific properties
            var entity = CustomerConverter<VesselOwnerCustomerEntity, VesselOwnerCustomerModel>.Convert(model);
            
            // Set VesselOwner-specific properties
            entity.LicenseNumber = model.LicenseNumber;
            entity.LicenseExpiryDate = model.LicenseExpiryDate;
            
            entity.ContactId = model.Contact.Id;
            
            return entity;
        }
    }

    // public class VesselOwnerCustomerModelToInputConverter 
    //     : IModelToInputConverter<VesselOwnerCustomerModel, VesselOwnerCustomerInputModel>
    // {
    //     public static VesselOwnerCustomerInputModel Convert(VesselOwnerCustomerModel model)
    //     {
    //         var input = CustomerConverter
    //     }
    //
    //     public static VesselOwnerCustomerModel Convert(VesselOwnerCustomerInputModel input)
    //     {
    //         throw new NotImplementedException();
    //     }
    // }
} 