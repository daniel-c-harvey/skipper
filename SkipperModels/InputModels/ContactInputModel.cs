using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Shared.InputModels;

namespace SkipperModels.InputModels
{
    public class ContactInputModel : InputModelBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public AddressInputModel Address { get; set; }
    }
}