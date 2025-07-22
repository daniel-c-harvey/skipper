using Models.Shared.InputModels;

namespace SkipperModels.InputModels
{
    public class MemberCustomerProfileInputModel : InputModelBase
    {
        public ContactInputModel Contact { get; set; }
        public DateTime? MembershipStartDate { get; set; }
        public DateTime? MembershipEndDate { get; set; }
        public string? MembershipLevel { get; set; }
    }
} 