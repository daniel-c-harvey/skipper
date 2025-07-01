namespace SkipperModels.Entities;

public class VesselEntity : BaseEntity<VesselEntity, VesselModel>, IEntity<VesselEntity, VesselModel>
{
    public string RegistrationNumber { get; set; }
    public string Name { get; set; }
    public decimal Length { get; set; }
    public decimal Beam { get; set; }
    public VesselType VesselType { get; set; }
    public static VesselModel CreateModel(VesselEntity entity)
    {
        return new VesselModel
        {
            Id = entity.Id,
            RegistrationNumber = entity.RegistrationNumber,
            Name = entity.Name,
            Length = entity.Length,
            Beam = entity.Beam,
            VesselType = entity.VesselType,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }
}

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