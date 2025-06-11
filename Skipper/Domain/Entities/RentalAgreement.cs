namespace Skipper.Domain.Entities;

public class RentalAgreement : BaseEntity
{
    public long SlipId { get; set; }
    public Slip Slip { get; set; }
    public long VesselId { get; set; }
    public Vessel Vessel { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PriceRate { get; set; }
    public PriceUnit PriceUnit { get; set; }
    public RentalStatus Status { get; set; }
}
