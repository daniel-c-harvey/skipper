using Models.Shared.InputModels;

namespace SkipperModels.InputModels
{
    public class VesselOwnerProfileInputModel : InputModelBase
    {
        public ContactInputModel Contact { get; set; }
        public ICollection<VesselInputModel> Vessels { get; set; }
    }
} 