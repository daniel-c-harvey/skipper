using Models.Shared.Models;

namespace SkipperModels.Models;

public class VesselModel : BaseModel, IModel
{
    public string RegistrationNumber { get; set; }
    public string Name { get; set; }
    public decimal Length { get; set; }
    public decimal Beam { get; set; }
    public VesselType VesselType { get; set; }
}