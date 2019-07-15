using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class Length : Result
    {
        [JsonConstructor]
        private Length(FLT flt, double value, LengthUnit unit)
            : base(flt, value, unit)
        {
        }
        
        public Length(double value, LengthUnit unit)
            : base(FLT.Length, NormalizeValue(value, unit), DefaultUnit)
        {
        }

        public static LengthUnit DefaultUnit => LengthUnit.Inch;

        public static Result CreateWithStandardUnits(double value)
        {
            return new Length(value, DefaultUnit);
        }

        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == LengthUnit.Inch) return Value;
            if (targetUnit == LengthUnit.Foot) return Value / Constants.InchesPerFoot;
            if (targetUnit == LengthUnit.Millimeter) return Value / Constants.InchesPerMeter * Constants.MillimetersPerMeter;
            if (targetUnit == LengthUnit.Meter) return Value / Constants.InchesPerMeter;
            
            throw new ArgumentException("cannot convert to the target unit");
        }
        
        // Normalized to inch
        protected static double NormalizeValue(double value, Unit unit)
        {
            if (unit == LengthUnit.Inch) return value;
            if (unit == LengthUnit.Foot) return value * Constants.InchesPerFoot;
            if (unit == LengthUnit.Millimeter) return value / Constants.MillimetersPerMeter * Constants.InchesPerMeter;
            if (unit == LengthUnit.Meter) return value * Constants.InchesPerMeter;
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToLaTexString(ConvertTo(DisplayUnit), DisplayUnit);
        }
    }
}