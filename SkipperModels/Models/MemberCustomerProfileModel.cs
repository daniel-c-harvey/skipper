using Models.Shared.Models;

namespace SkipperModels.Models
{
    public class MemberCustomerProfileModel : BaseModel, IModel
    {
        public ContactModel Contact { get; set; }
        public DateTime? MembershipStartDate { get; set; }
        public DateTime? MembershipEndDate { get; set; }
        public string? MembershipLevel { get; set; }
    }
} 