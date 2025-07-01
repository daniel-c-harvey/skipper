namespace SkipperModels.Entities;

public class SlipEntity : BaseEntity<SlipEntity, SlipModel>, IEntity<SlipEntity, SlipModel>
{
    public long SlipClassificationId { get; set; }
    public virtual SlipClassificationEntity SlipClassificationEntity { get; set; }
    public string SlipNumber { get; set; }
    public string LocationCode { get; set; }
    public SlipStatus Status { get; set; }
    
    // Interface method - required by IEntity
    public static SlipModel CreateModel(SlipEntity entity)
    {
        return new SlipModel()
        {
            Id = entity.Id,
            SlipClassification = SlipClassificationEntity.CreateModel(entity.SlipClassificationEntity),
            SlipNumber = entity.SlipNumber,
            LocationCode = entity.LocationCode,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }
    
    internal static SlipModel CreateModel(SlipEntity entity, SlipClassificationModel slipClassification)
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
}

public class SlipModel : BaseModel<SlipModel, SlipEntity>, IModel<SlipModel, SlipEntity>
{
    public virtual SlipClassificationModel SlipClassification { get; set; }
    public string SlipNumber { get; set; }
    public string LocationCode { get; set; }
    public SlipStatus Status { get; set; }
    
    // Interface method - required by IModel
    public static SlipEntity CreateEntity(SlipModel model)
    {
        return new SlipEntity()
        {
            Id = model.Id,
            SlipClassificationId = model.SlipClassification?.Id ?? 0,
            SlipClassificationEntity = SlipClassificationModel.CreateEntity(model.SlipClassification),
            SlipNumber = model.SlipNumber,
            LocationCode = model.LocationCode,
            Status = model.Status,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }
    
    // Depth-controlled method
    public static SlipEntity CreateEntity(SlipModel model, SlipClassificationEntity slipClassification)
    {
        return new SlipEntity()
        {
            Id = model.Id,
            SlipClassificationId = model.SlipClassification?.Id ?? 0,
            SlipClassificationEntity = slipClassification,
            SlipNumber = model.SlipNumber,
            LocationCode = model.LocationCode,
            Status = model.Status,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }
}
