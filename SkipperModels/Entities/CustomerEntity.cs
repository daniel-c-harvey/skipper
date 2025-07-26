using Models.Shared.Entities;

namespace SkipperModels.Entities
{
    public abstract class CustomerEntity<TCustomerProfile> : BaseEntity, IEntity
    where TCustomerProfile : CustomerProfileBaseEntity
    {
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public CustomerProfileType CustomerProfileType { get; set; }
        public long CustomerProfileId { get; set; }
        public virtual TCustomerProfile CustomerProfile { get; set; }
    }
}