namespace SkipperModels.Entities;

public class MemberCustomerEntity : CustomerEntity
{
    // Member-specific properties (can be extended as needed)
    public string? MembershipNumber { get; set; }
    public DateTime? MemberSince { get; set; }
    public string? MembershipLevel { get; set; }

    public MemberCustomerEntity()
    {
        CustomerProfileType = CustomerProfileType.MemberCustomerProfile;
    }
} 