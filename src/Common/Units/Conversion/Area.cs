using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class Area : Result
    {
        [JsonConstructor]
        private Area(FLT flt, double value, AreaUnit unit)
            : base(flt, value, unit)
        {
        }
        
        public Area(double value, AreaUnit unit)
            : base(FLT.Area, NormalizeValue(value, unit), DefaultUnit)
        {
        }

        public static AreaUnit DefaultUnit => AreaUnit.SquareInch;

        public static Result CreateWithStandardUnits(double value)
        {
            return new Area(value, DefaultUnit);
        }

        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == AreaUnit.SquareInch) return Value;
            if (targetUnit == AreaUnit.SquareFoot) return Value / (Constants.InchesPerFoot * Constants.InchesPerFoot);
            if (targetUnit == AreaUnit.SquareMillimeter) return Value / (Constants.InchesPerMeter * Constants.InchesPerMeter) * (Constants.MillimetersPerMeter * Constants.MillimetersPerMeter);
            if (targetUnit == AreaUnit.SquareMeter) return Value / (Constants.InchesPerMeter * Constants.InchesPerMeter);
            
            throw new ArgumentException("cannot convert to the target unit");
        }

        // Normalized to square inch
        protected static double NormalizeValue(double value, Unit unit)
        {
            
            if (unit == AreaUnit.SquareInch) return value;
            if (unit == AreaUnit.SquareFoot) return value * (Constants.InchesPerFoot * Constants.InchesPerFoot);
            if (unit == AreaUnit.SquareMillimeter) return value / (Constants.MillimetersPerMeter * Constants.MillimetersPerMeter) * (Constants.InchesPerMeter * Constants.InchesPerMeter);
            if (unit == AreaUnit.SquareMeter) return value * (Constants.InchesPerMeter * Constants.InchesPerMeter);
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToLaTexString(ConvertTo(DisplayUnit), DisplayUnit);
        }
    }
}