using Models.Shared.Entities;
using Models.Shared;

namespace SkipperModels.Entities
{
    public abstract class CustomerEntity : BaseEntity, IEntity
    {
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public CustomerProfileType CustomerProfileType { get; set; } // TPH Discriminator
    }
}