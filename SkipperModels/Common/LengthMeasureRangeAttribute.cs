using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace SkipperModels.Common
{
    public class LengthMeasureRangeAttribute : RangeAttribute
    {
        public LengthMeasureRangeAttribute(double minimum, double maximum, string name) 
        : base(minimum, maximum)
        {
            ErrorMessage = $"{name} must be greater than {minimum} and less than {maximum}";
        }
    }
    public class LengthRangeAttribute : LengthMeasureRangeAttribute
    {
        private static readonly double MINIMUM = 1D;
        private static readonly double MAXIMUM = 250D;

        public LengthRangeAttribute() 
        : base(MINIMUM, MAXIMUM, "Length") { }
    }
    
    public class BeamRangeAttribute : LengthMeasureRangeAttribute
    {
        private static readonly double MINIMUM = 1D;
        private static readonly double MAXIMUM = 80D;

        public BeamRangeAttribute() 
        : base(MINIMUM, MAXIMUM, "Beam") { }
    }
}
