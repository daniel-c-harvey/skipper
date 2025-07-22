using Models.Shared.Entities;

namespace SkipperModels.Entities
{
    public class CustomerEntity : BaseEntity, IEntity
    {
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public CustomerProfileType CustomerProfileType { get; set; }
        
        // Polymorphic association to customer details
        public long CustomerProfileId { get; set; }
    }
}