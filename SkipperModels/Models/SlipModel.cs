using Models.Shared.Models;

namespace SkipperModels.Models;

public class SlipModel : BaseModel, IModel
{
    public virtual SlipClassificationModel SlipClassification { get; set; }
    public string SlipNumber { get; set; }
    public string LocationCode { get; set; }
    public SlipStatus Status { get; set; }
}