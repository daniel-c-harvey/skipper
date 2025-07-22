using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Shared.Models;
using SkipperModels;

namespace SkipperModels.Models
{
    public class CustomerModel : BaseModel, IModel
    {
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public CustomerProfileType CustomerProfileType { get; set; }
        public long CustomerProfileId { get; set; }
    }
}