using Models.Shared.Models;
using SkipperModels.Entities;

namespace SkipperModels.Models;

public class VesselModel : BaseModel<VesselModel, VesselEntity>, IModel<VesselModel, VesselEntity>
{
    public string RegistrationNumber { get; set; }
    public string Name { get; set; }
    public decimal Length { get; set; }
    public decimal Beam { get; set; }
    public VesselType VesselType { get; set; }
    public static VesselEntity CreateEntity(VesselModel model)
    {
        return new VesselEntity()
        {
            Id = model.Id,
            RegistrationNumber = model.RegistrationNumber,
            Name = model.Name,
            Length = model.Length,
            Beam = model.Beam,
            VesselType = model.VesselType,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }
}