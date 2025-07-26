using Models.Shared.Entities;

namespace SkipperModels.Entities
{
    public class AddressEntity : BaseEntity, IEntity
    {
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}