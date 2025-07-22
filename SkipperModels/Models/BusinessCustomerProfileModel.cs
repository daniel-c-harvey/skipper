using Models.Shared.Models;

namespace SkipperModels.Models
{
    public class BusinessCustomerProfileModel : BaseModel, IModel
    {
        public string BusinessName { get; set; }
        public string? TaxId { get; set; }
        public ICollection<ContactModel>? Contacts { get; set; }
    }
} 