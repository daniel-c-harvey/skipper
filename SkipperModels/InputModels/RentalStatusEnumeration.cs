using NetBlocks.Utilities;

namespace SkipperModels.InputModels;

public class RentalStatusEnumeration : DisplayEnumeration<RentalStatusEnumeration>
{
    public static RentalStatusEnumeration Quoted = new(RentalStatus.Quoted, "Quoted");
    public static RentalStatusEnumeration Pending = new(RentalStatus.Pending, "Pending");
    public static RentalStatusEnumeration Active = new(RentalStatus.Active, "Active");
    public static RentalStatusEnumeration Expired = new(RentalStatus.Expired, "Expired");
    public static RentalStatusEnumeration Cancelled = new(RentalStatus.Cancelled, "Cancelled");
    
    public RentalStatus RentalStatus { get; init; }
    
    private RentalStatusEnumeration(RentalStatus rentalStatus, string disaplyName) : base((int)rentalStatus, rentalStatus.ToString("G"), disaplyName)
    {
    }
}