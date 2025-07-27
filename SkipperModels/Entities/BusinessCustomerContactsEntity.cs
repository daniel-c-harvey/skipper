using Models.Shared.Entities;

namespace SkipperModels.Entities;

public class BusinessCustomerContactsEntity : BaseLinkageEntity
{
    public bool IsPrimary { get; set; }
    public bool IsEmergency { get; set; }
    public long BusinessCustomerId { get; set; }
    public virtual BusinessCustomerEntity BusinessCustomer { get; set; }
    public long ContactId { get; set; }
    public virtual ContactEntity Contact { get; set; }
}