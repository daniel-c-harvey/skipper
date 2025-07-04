using SkipperModels.Entities;

namespace SkipperModels.Models;

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
            // SlipClassificationEntity = SlipClassificationModel.CreateEntity(model.SlipClassification),
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
            // SlipClassificationEntity = slipClassification,
            SlipNumber = model.SlipNumber,
            LocationCode = model.LocationCode,
            Status = model.Status,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }
}