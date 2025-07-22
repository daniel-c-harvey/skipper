using Models.Shared.Entities;

namespace SkipperModels.Entities
{
    public class MemberCustomerProfileEntity : BaseEntity, ICustomerProfile
    {
        public long ContactId { get; set; }
        public virtual ContactEntity Contact { get; set; }
        
        // Member-specific fields
        public DateTime? MembershipStartDate { get; set; }
        public DateTime? MembershipEndDate { get; set; }
        public string? MembershipLevel { get; set; } // Bronze, Silver, Gold, etc.
        
        public ContactEntity GetPrimaryContact() => Contact;
    }
} 