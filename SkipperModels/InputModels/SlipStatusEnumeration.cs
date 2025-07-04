using NetBlocks.Utilities;

namespace SkipperModels.InputModels;

public class SlipStatusEnumeration : DisplayEnumeration<SlipStatusEnumeration>
{
    public static SlipStatusEnumeration Available = new(SlipStatus.Available, "Available", "Available");
    public static SlipStatusEnumeration Booked = new(SlipStatus.Booked, "Booked", "Booked");
    public static SlipStatusEnumeration InUse = new(SlipStatus.InUse, "In Use", "In Use");
    public static SlipStatusEnumeration Maintenance = new(SlipStatus.Maintenance, "Maintenance", "Maintenance");
    public static SlipStatusEnumeration Sold = new(SlipStatus.Sold, "Sold", "Sold");
    public static SlipStatusEnumeration Archived = new(SlipStatus.Archived, "Archived", "Archived");

    public SlipStatus SlipStatus { get; init; }

    private SlipStatusEnumeration(SlipStatus status, string name, string displayValue) : base((int)status, name, displayValue) 
    {
        SlipStatus = status;
    }
}