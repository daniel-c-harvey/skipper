namespace SkipperModels.Entities;

public class VesselOwnerCustomerEntity : CustomerEntity
{
    // Vessel owner specific properties (from old VesselOwnerProfileEntity)
    public string? LicenseNumber { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
    
    // Direct contact relationship
    public long ContactId { get; set; }
    public virtual ContactEntity Contact { get; set; }
    
    // Direct vessel relationships (from old VesselOwnerProfileEntity)
    public virtual ICollection<VesselOwnerVesselEntity> VesselOwnerVessels { get; set; } = new List<VesselOwnerVesselEntity>();

    public VesselOwnerCustomerEntity()
    {
        CustomerProfileType = CustomerProfileType.VesselOwnerProfile;
    }
}