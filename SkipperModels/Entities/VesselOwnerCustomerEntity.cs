namespace SkipperModels.Entities;

public class VesselOwnerCustomerEntity : CustomerEntity<VesselOwnerProfileEntity>
{
    // Vessel owner specific properties
    public string LicenseNumber { get; set; }
    public DateTime LicenseExpiryDate { get; set; }
}