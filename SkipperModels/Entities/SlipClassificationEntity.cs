using SkipperModels.Models;

namespace SkipperModels.Entities;

public class SlipClassificationEntity : BaseEntity<SlipClassificationEntity, SlipClassificationModel>, IEntity<SlipClassificationEntity, SlipClassificationModel>
{
    public string Name { get; set; }
    public decimal MaxLength { get; set; }
    public decimal MaxBeam { get; set; }
    public int BasePrice { get; set; }
    public string Description { get; set; }
    public virtual ICollection<SlipEntity> Slips { get; set; }
    public static SlipClassificationModel CreateModel(SlipClassificationEntity entity)
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
        model.Slips = entity.Slips.Select(slip => SlipEntity.CreateModel(slip, model)).ToList();
        return model;
    }
}
