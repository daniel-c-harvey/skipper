using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters;

public class SlipReservationEntityToModelConverter : IEntityToModelConverter<SlipReservationEntity, SlipReservationModel>
{
    public static SlipReservationModel Convert(SlipReservationEntity entity)
    {
        return new SlipReservationModel()
        {
            Id = entity.Id,
            Slip = SlipEntityToModelConverter.Convert(entity.SlipEntity),
            Vessel = VesselEntityToModelConverter.Convert(entity.VesselEntity),
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            PriceRate = entity.PriceRate / 100M,
            PriceUnit = entity.PriceUnit,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }

    public static SlipReservationEntity Convert(SlipReservationModel model)
    {
        return new SlipReservationEntity()
        {
            Id = model.Id,
            SlipId = model.Slip.Id,
            // SlipEntity = SlipModel.CreateEntity(model.Slip),
            VesselId = model.Vessel.Id,
            // VesselEntity = VesselModel.CreateEntity(model.Vessel),
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            PriceRate = (int)Math.Round(model.PriceRate * 100M, 0), // convert from dollars for front end to cents for data transfer.
            PriceUnit = model.PriceUnit,
            Status = model.Status,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }
}

public class SlipReservationModelToInputConverter : IModelToInputConverter<SlipReservationModel, SlipReservationInputModel>
{
    public static SlipReservationModel Convert(SlipReservationInputModel input)
    {
        return new SlipReservationModel()
        {
            Id = input.Id,
            Slip = SlipModelToInputConverter.Convert(input.Slip!),
            Vessel = VesselModelToInputConverter.Convert(input.Vessel!),
            StartDate = input.StartDate ?? default,
            EndDate = input.EndDate ?? default,
            PriceRate = input.PriceRate,
            PriceUnit = input.PriceUnit!.PriceUnit,
            Status = input.Status!.RentalStatus,
            CreatedAt = input.CreatedAt,
            UpdatedAt = input.UpdatedAt
        };
    }

    public static SlipReservationInputModel Convert(SlipReservationModel model)
    {
        return new SlipReservationInputModel()
        {
            Id = model.Id,
            Slip = SlipModelToInputConverter.Convert(model.Slip),
            Vessel = VesselModelToInputConverter.Convert(model.Vessel),
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            PriceRate = model.PriceRate,
            PriceUnit = PriceUnitEnumeration.GetById((int)model.PriceUnit)!,
            Status = RentalStatusEnumeration.GetById((int)model.Status)!,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }
} 