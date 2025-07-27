using Models.Shared.Models;

namespace SkipperModels.Models
{
    public class SlipReservationOrderModel : OrderModel
    {
        // Slip-specific properties
        public long SlipId { get; set; }
        public SlipModel? Slip { get; set; }
        public long VesselId { get; set; }
        public VesselModel? Vessel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PriceRate { get; set; }
        public PriceUnit PriceUnit { get; set; }
        public RentalStatus RentalStatus { get; set; }
    }
} 