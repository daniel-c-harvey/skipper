namespace SkipperModels.Entities;

public class SlipClassification : BaseEntity
{
    public string Name { get; set; }
    public decimal MaxLength { get; set; }
    public decimal MaxBeam { get; set; }
    public int BasePrice { get; set; }
    public string Description { get; set; }
}
