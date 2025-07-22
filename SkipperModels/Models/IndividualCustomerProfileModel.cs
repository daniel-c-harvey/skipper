using Models.Shared.Models;

namespace SkipperModels.Models
{
    public class IndividualCustomerProfileModel : BaseModel, IModel
    {
        public ContactModel Contact { get; set; }
    }
} 