using Models.Shared.InputModels;

namespace SkipperModels.InputModels
{
    public class BusinessCustomerProfileInputModel : InputModelBase
    {
        public string BusinessName { get; set; }
        public string? TaxId { get; set; }
        public ICollection<ContactInputModel>? Contacts { get; set; }
    }
} 