using Models.Shared.Models;

namespace SkipperModels.Models
{
    public class SlipReservationOrderModel : OrderModel<VesselOwnerCustomerModel>
    {
        // Slip-specific properties
        public SlipModel Slip { get; set; }
        public VesselModel Vessel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PriceRate { get; set; }
        public PriceUnit PriceUnit { get; set; }
        public RentalStatus RentalStatus { get; set; }
    }
} 