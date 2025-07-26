using Models.Shared.Entities;

namespace SkipperModels.Entities;

public abstract class CustomerProfileBaseEntity : BaseEntity, ICustomerProfile
{
    public abstract ContactEntity GetPrimaryContact();
}