namespace SkipperModels
{
    /// <summary>
    /// Defines the type of customer profile for polymorphic customer relationships
    /// </summary>
    public enum CustomerProfileType
    {
        VesselOwnerProfile,           // Links to VesselOwnerProfileEntity
        IndividualCustomerProfile,    // Links to IndividualCustomerProfileEntity  
        BusinessCustomerProfile,      // Links to BusinessCustomerProfileEntity
        MemberCustomerProfile         // Links to MemberCustomerProfileEntity
    }
}