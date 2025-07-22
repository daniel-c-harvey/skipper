using Models.Shared.Models;

namespace SkipperModels.Models
{
    public class VesselOwnerProfileModel : BaseModel, IModel
    {
        public ContactModel Contact { get; set; }
        public ICollection<VesselModel> Vessels { get; set; }
    }
} 