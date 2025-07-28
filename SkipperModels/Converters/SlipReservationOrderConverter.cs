using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperModels.Converters;

public class SlipReservationOrderConverter : IEntityToModelConverter<SlipReservationOrderEntity, SlipReservationOrderModel>
{
    public static SlipReservationOrderModel Convert(SlipReservationOrderEntity entity)
    {
        // Use the generic base converter to set base properties, then add specific properties
        var model = OrderConverter<SlipReservationOrderEntity, SlipReservationOrderModel, VesselOwnerCustomerEntity, VesselOwnerCustomerModel>.Convert(entity);
        
        // Set SlipReservation-specific properties
        model.Slip = SlipEntityToModelConverter.Convert(entity.SlipEntity);
        model.Vessel = VesselEntityToModelConverter.Convert(entity.VesselEntity);
        model.StartDate = entity.StartDate;
        model.EndDate = entity.EndDate;
        model.PriceRate = entity.PriceRate;
        model.PriceUnit = entity.PriceUnit;
        model.RentalStatus = entity.RentalStatus;
        
        return model;
    }

    public static SlipReservationOrderEntity Convert(SlipReservationOrderModel model)
    {
        // Use the generic base converter to set base properties, then add specific properties
        var entity = OrderConverter<SlipReservationOrderEntity, SlipReservationOrderModel, VesselOwnerCustomerEntity, VesselOwnerCustomerModel>.Convert(model);
        
        // Set SlipReservation-specific properties
        entity.SlipId = model.Slip.Id;
        entity.VesselId = model.Vessel.Id;
        entity.StartDate = model.StartDate;
        entity.EndDate = model.EndDate;
        entity.PriceRate = model.PriceRate;
        entity.PriceUnit = model.PriceUnit;
        entity.RentalStatus = model.RentalStatus;
        
        return entity;
    }
}