using Models.Shared.Entities;

namespace SkipperModels.Entities
{
    public class VesselOwnerProfileEntity : BaseEntity, ICustomerProfile
    {
        public long ContactId { get; set; }
        public virtual ContactEntity Contact { get; set; }
        public virtual ICollection<VesselOwnerVesselEntity> VesselOwnerVessels { get; set; }
        
        public ContactEntity GetPrimaryContact() => Contact;
    }
} 