namespace SkipperModels.Entities;

public class BusinessCustomerEntity : CustomerEntity
{
    // Business-specific properties (from old BusinessCustomerProfileEntity)
    public string BusinessName { get; set; }
    public string? TaxId { get; set; }
    
    // Direct business contacts relationship (from old BusinessCustomerProfileEntity)
    public virtual ICollection<BusinessCustomerContactsEntity> BusinessCustomerContacts { get; set; } = new List<BusinessCustomerContactsEntity>();

    public BusinessCustomerEntity()
    {
        CustomerProfileType = CustomerProfileType.BusinessCustomerProfile;
    }
} 