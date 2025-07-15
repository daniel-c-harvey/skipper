using Models.Shared.Entities;

namespace SkipperModels.Entities;

public class VesselEntity : BaseEntity, IEntity
{
    public string RegistrationNumber { get; set; }
    public string Name { get; set; }
    public decimal Length { get; set; }
    public decimal Beam { get; set; }
    public VesselType VesselType { get; set; }
}