using Models.Shared.Models;

namespace SkipperModels.Models;


public class SlipReservationModel : BaseModel, IModel
{
    public SlipModel Slip { get; set; }
    public VesselModel Vessel { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PriceRate { get; set; }
    public PriceUnit PriceUnit { get; set; }
    public RentalStatus Status { get; set; }
} 