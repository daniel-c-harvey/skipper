using Models.Shared.Entities;

namespace SkipperModels.Entities;

public class BusinessCustomerContactsEntity : BaseLinkageEntity
{
    public bool IsPrimary { get; set; }
    public bool IsEmergency { get; set; }
    public long BusinessCustomerProfileId { get; set; }
    public virtual BusinessCustomerProfileEntity BusinessCustomerProfile { get; set; }
    public long ContactId { get; set; }
    public virtual ContactEntity Contact { get; set; }
}