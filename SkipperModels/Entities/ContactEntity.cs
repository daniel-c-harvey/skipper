using Models.Shared.Entities;

namespace SkipperModels.Entities
{
    public class ContactEntity : BaseEntity, IEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public long AddressId { get; set; }
        public virtual AddressEntity Address { get; set; }
    }
}