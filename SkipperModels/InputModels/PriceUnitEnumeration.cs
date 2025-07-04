using NetBlocks.Utilities;

namespace SkipperModels.InputModels;

public class PriceUnitEnumeration : DisplayEnumeration<PriceUnitEnumeration>
{
    public static PriceUnitEnumeration PerDay = new(PriceUnit.PerDay, "Per Day");
    public static PriceUnitEnumeration PerWeek = new(PriceUnit.PerWeek, "Per Week");
    public static PriceUnitEnumeration PerMonth = new(PriceUnit.PerMonth, "Per Month");
    public static PriceUnitEnumeration PerYear = new(PriceUnit.PerYear, "Per Year");
    
    public PriceUnit PriceUnit { get; init; }
    
    private PriceUnitEnumeration(PriceUnit priceUnit, string disaplyName) : base((int)priceUnit, priceUnit.ToString("G"), disaplyName) 
    {
        PriceUnit = priceUnit;
    }
}