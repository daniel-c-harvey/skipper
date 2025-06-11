namespace SkipperModels.Entities;

public class Slip : BaseEntity
{
    public long SlipClassificationId { get; set; }
    public SlipClassification SlipClassification { get; set; }
    public string SlipNumber { get; set; }
    public string LocationCode { get; set; }
    public SlipStatus Status { get; set; }
}
