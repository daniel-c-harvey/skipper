using Models.Shared.Entities;

namespace SkipperModels.Entities
{
    public class IndividualCustomerProfileEntity : BaseEntity, ICustomerProfile
    {
        public long ContactId { get; set; }
        public virtual ContactEntity Contact { get; set; }
        
        public ContactEntity GetPrimaryContact() => Contact;
    }
} 