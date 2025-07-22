using Models.Shared.Models;

namespace SkipperModels.Models
{
    public class ContactModel : BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public AddressModel Address { get; set; }
    }
}