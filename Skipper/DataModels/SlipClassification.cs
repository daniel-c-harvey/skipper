using DataBlocks.Migrations;

namespace Skipper.DataModels;

[ScheModel]
public class SlipClassification
{
    public int ClassificationId { get; set; }
    public string SlipName { get; set; }
    public decimal MaxLength { get; set; }
    public decimal MaxBeam { get; set; }
    public decimal BasePrice { get; set; }
    public string Description { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}