using Models.Shared.Converters;
using SkipperModels.Entities;
using SkipperModels.InputModels;
using SkipperModels.Models;

namespace SkipperModels.Converters;

public class SlipEntityToModelConverter : IEntityToModelConverter<SlipEntity, SlipModel>
{
    public static SlipModel Convert(SlipEntity entity)
    {
        return new SlipModel()
        {
            Id = entity.Id,
            SlipClassification = SlipClassificationEntityToModelConverter.Convert(entity.SlipClassificationEntity),
            SlipNumber = entity.SlipNumber,
            LocationCode = entity.LocationCode,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }
    
    public static SlipModel Convert(SlipEntity entity, SlipClassificationModel slipClassification)
    {
        return new SlipModel()
        {
            Id = entity.Id,
            SlipClassification = slipClassification,
            SlipNumber = entity.SlipNumber,
            LocationCode = entity.LocationCode,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }

    public static SlipEntity Convert(SlipModel model)
    {
        return new SlipEntity()
        {
            Id = model.Id,
            SlipClassificationId = model.SlipClassification?.Id ?? 0,
            // SlipClassificationEntity = SlipClassificationModel.CreateEntity(model.SlipClassification),
            SlipNumber = model.SlipNumber,
            LocationCode = model.LocationCode,
            Status = model.Status,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }
    
    // Depth-controlled method
    public static SlipEntity Convert(SlipModel model, SlipClassificationEntity slipClassification)
    {
        return new SlipEntity()
        {
            Id = model.Id,
            SlipClassificationId = model.SlipClassification?.Id ?? 0,
            // SlipClassificationEntity = slipClassification,
            SlipNumber = model.SlipNumber,
            LocationCode = model.LocationCode,
            Status = model.Status,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }
}

public class SlipModelToInputConverter : IModelToInputConverter<SlipModel, SlipInputModel>
{
    public static SlipInputModel Convert(SlipModel model)
    {
        return new SlipInputModel()
        {
            Id = model.Id,
            SlipNumber = model.SlipNumber,
            SlipClassification = SlipClassificationModelToInputConverter.Convert(model.SlipClassification),
            LocationCode = model.LocationCode,
            Status = SlipStatusEnumeration.GetById((int)model.Status)!,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
            
        };
    }

    public static SlipModel Convert(SlipInputModel input)
    {
        return new SlipModel()
        {
            Id = input.Id,
            SlipClassification = SlipClassificationModelToInputConverter.Convert(input.SlipClassification),
            SlipNumber = input.SlipNumber,
            LocationCode = input.LocationCode,
            Status = input.Status.SlipStatus,
            CreatedAt = input.CreatedAt,
            UpdatedAt = input.UpdatedAt
        };
    }
}