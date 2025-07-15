using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters;

public class RentalAgreementEntityToModelConverter : IEntityToModelConverter<RentalAgreementEntity, RentalAgreementModel>
{
    public static RentalAgreementModel Convert(RentalAgreementEntity entity)
    {
        return new RentalAgreementModel()
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

    public static RentalAgreementEntity Convert(RentalAgreementModel model)
    {
        return new RentalAgreementEntity()
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

public class RentalAgreementModelToInputConverter : IModelToInputConverter<RentalAgreementModel, RentalAgreementInputModel>
{
    public static RentalAgreementModel Convert(RentalAgreementInputModel input)
    {
        return new RentalAgreementModel()
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

    public static RentalAgreementInputModel Convert(RentalAgreementModel model)
    {
        return new RentalAgreementInputModel()
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