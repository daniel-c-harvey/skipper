using Models.Shared.Models;

namespace SkipperModels.Models;

public class SlipClassificationModel : BaseModel, IModel
{
    public string Name { get; set; }
    public decimal MaxLength { get; set; }
    public decimal MaxBeam { get; set; }
    public int BasePrice { get; set; }
    public string Description { get; set; }
    public virtual ICollection<SlipModel>? Slips { get; set; }
}