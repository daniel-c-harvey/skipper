using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters;

public class SlipClassificationEntityToModelConverter : IEntityToModelConverter<SlipClassificationEntity, SlipClassificationModel>
{
    public static SlipClassificationModel Convert(SlipClassificationEntity entity)
    {
        var model = new SlipClassificationModel
        {
            Id = entity.Id,
            Name = entity.Name,
            MaxLength = entity.MaxLength,
            MaxBeam = entity.MaxBeam,
            BasePrice = entity.BasePrice,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
        model.Slips = entity.Slips.Select(slip => SlipEntityToModelConverter.Convert(slip, model)).ToList();
        return model;
    }

    public static SlipClassificationEntity Convert(SlipClassificationModel model)
    {
        var entity = new SlipClassificationEntity
        {
            Id = model.Id,
            Name = model.Name,
            MaxLength = model.MaxLength,
            MaxBeam = model.MaxBeam,
            BasePrice = model.BasePrice,
            Description = model.Description,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
        // Do not set the navigation properties when creating entities.
        // entity.Slips = model.Slips?.Select(slip => SlipModel.CreateEntity(slip, entity))?.ToList() ?? [];
        return entity;
    }
}

public class SlipClassificationModelToInputConverter : IModelToInputConverter<SlipClassificationModel, SlipClassificationInputModel>
{
    public static SlipClassificationInputModel Convert(SlipClassificationModel model)
    {
        return new SlipClassificationInputModel()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            BasePrice = model.BasePrice / 100M,
            MaxLength = model.MaxLength,
            MaxBeam = model.MaxBeam,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }

    public static SlipClassificationModel Convert(SlipClassificationInputModel input)
    {
        return new SlipClassificationModel()
        {
            Id = input.Id,
            Name = input.Name,
            Description = input.Description,
            BasePrice = (int)Math.Round(input.BasePrice * 100, 0), // convert from dollars for front end to cents for data transfer.
            MaxLength = input.MaxLength,
            MaxBeam = input.MaxBeam,
            CreatedAt = input.CreatedAt,
            UpdatedAt = input.UpdatedAt
        };
    }
}