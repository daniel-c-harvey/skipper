using Models.Shared.InputModels;
using SkipperModels;

namespace SkipperModels.InputModels
{
    public class CustomerInputModel : InputModelBase
    {
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public CustomerProfileType CustomerProfileType { get; set; }
    }
} 