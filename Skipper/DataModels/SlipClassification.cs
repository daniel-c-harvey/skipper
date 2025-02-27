using DataBlocks.Migrations;

namespace Skipper.DataModels;

[ScheModel]
public class SlipClassification
{
    [ScheKey("classification_id")]
    public int ClassificationId { get; set; }
    [ScheData("slip_name")]
    public string SlipName { get; set; }
    [ScheData("max_length")]
    public decimal MaxLength { get; set; }
    [ScheData("max_beam")]
    public decimal MaxBeam { get; set; }
    [ScheData("base_price")]
    public decimal BasePrice { get; set; }
    [ScheData("description")]
    public string Description { get; set; }
}