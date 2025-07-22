using Models.Shared.Entities;

namespace SkipperModels.Entities
{
    public class BusinessCustomerProfileEntity : BaseEntity, ICustomerProfile
    {
        public string BusinessName { get; set; }
        public string? TaxId { get; set; }
        public virtual ICollection<BusinessCustomerContactsEntity> BusinessCustomerContacts { get; set; }
        
        public ContactEntity GetPrimaryContact() => BusinessCustomerContacts.First(bc => bc.IsPrimary).Contact;
    }
} 