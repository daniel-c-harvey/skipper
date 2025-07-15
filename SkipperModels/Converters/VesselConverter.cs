using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters;

public class VesselEntityToModelConverter : IEntityToModelConverter<VesselEntity, VesselModel>
{
    public static VesselModel Convert(VesselEntity entity)
    {
        return new VesselModel
        {
            Id = entity.Id,
            RegistrationNumber = entity.RegistrationNumber,
            Name = entity.Name,
            Length = entity.Length,
            Beam = entity.Beam,
            VesselType = entity.VesselType,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }

    public static VesselEntity Convert(VesselModel model)
    {
        return new VesselEntity()
        {
            Id = model.Id,
            RegistrationNumber = model.RegistrationNumber,
            Name = model.Name,
            Length = model.Length,
            Beam = model.Beam,
            VesselType = model.VesselType,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }
}

public class VesselModelToInputConverter : IModelToInputConverter<VesselModel, VesselInputModel>
{
    public static VesselInputModel Convert(VesselModel model)
    {
        return new VesselInputModel()
        {
            Id = model.Id,
            RegistrationNumber = model.RegistrationNumber,
            Name = model.Name,
            Length = model.Length,
            Beam = model.Beam,
            VesselType = model.VesselType,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }

    public static VesselModel Convert(VesselInputModel input)
    {
        return new VesselModel()
        {
            Id = input.Id,
            RegistrationNumber = input.RegistrationNumber,
            Name = input.Name,
            Length = input.Length,
            Beam = input.Beam,
            VesselType = input.VesselType,
            CreatedAt = input.CreatedAt,
            UpdatedAt = input.UpdatedAt
        };
    }
}