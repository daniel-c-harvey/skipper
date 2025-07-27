namespace SkipperModels.Entities;

public class IndividualCustomerEntity : CustomerEntity
{
    // Individual-specific properties (can be extended as needed)
    public DateTime? DateOfBirth { get; set; }
    public string? PreferredContactMethod { get; set; }

    public IndividualCustomerEntity()
    {
        CustomerProfileType = CustomerProfileType.IndividualCustomerProfile;
    }
} 