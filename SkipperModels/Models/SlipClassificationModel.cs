using Models.Shared.Models;
using SkipperModels.Entities;

namespace SkipperModels.Models;

public class SlipClassificationModel : BaseModel<SlipClassificationModel, SlipClassificationEntity>, IModel<SlipClassificationModel, SlipClassificationEntity>
{
    public string Name { get; set; }
    public decimal MaxLength { get; set; }
    public decimal MaxBeam { get; set; }
    public int BasePrice { get; set; }
    public string Description { get; set; }
    public virtual ICollection<SlipModel>? Slips { get; set; }
    public static SlipClassificationEntity CreateEntity(SlipClassificationModel model)
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