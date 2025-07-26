using Models.Shared.Entities;

namespace SkipperModels.Entities;

public class SlipClassificationEntity : BaseEntity, IEntity
{
    public string Name { get; set; }
    public decimal MaxLength { get; set; }
    public decimal MaxBeam { get; set; }
    public int BasePrice { get; set; }
    public string Description { get; set; }
}
