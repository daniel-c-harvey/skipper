using Models.Shared.Entities;

namespace SkipperModels.Entities;

public class SlipEntity : BaseEntity, IEntity
{
    public long SlipClassificationId { get; set; }
    public virtual SlipClassificationEntity SlipClassificationEntity { get; set; }
    public string SlipNumber { get; set; }
    public string LocationCode { get; set; }
    public SlipStatus Status { get; set; }
}
