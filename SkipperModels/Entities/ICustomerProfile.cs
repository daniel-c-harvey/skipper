using Models.Shared.Entities;

namespace SkipperModels.Entities
{
    public interface ICustomerProfile : IEntity
    {
        ContactEntity GetPrimaryContact();
    }
} 