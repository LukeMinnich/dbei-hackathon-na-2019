using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class LengthToThe6th : Result
    {
        [JsonConstructor]
        private LengthToThe6th(FLT flt, double value, LengthToThe6thUnit unit)
            : base(flt, value, unit)
        {
        }
        
        public LengthToThe6th(double value, LengthToThe6thUnit unit)
            : base(FLT.LengthToThe6th , NormalizeValue(value, unit), DefaultUnit)
        {
        }

        public static LengthToThe6thUnit DefaultUnit => LengthToThe6thUnit.InchesToThe6th;

        public static Result CreateWithStandardUnits(double value)
        {
            return new LengthToThe6th(value, DefaultUnit);
        }

        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == LengthToThe6thUnit.InchesToThe6th) return Value;
            if (targetUnit == LengthToThe6thUnit.FeetToThe6th) return Value / Math.Pow(Constants.InchesPerFoot, 6);
            if (targetUnit == LengthToThe6thUnit.MillimetersToThe6th) return Value / Math.Pow(Constants.InchesPerMeter, 6) * Math.Pow(Constants.MillimetersPerMeter, 6);
            if (targetUnit == LengthToThe6thUnit.MetersToThe6th) return Value / Math.Pow(Constants.InchesPerMeter, 6);
            
            throw new ArgumentException("cannot convert to the target unit");
        }

        protected static double NormalizeValue(double value, Unit unit)
        {
            if (unit == LengthToThe6thUnit.InchesToThe6th) return value;
            if (unit == LengthToThe6thUnit.FeetToThe6th) return value * Math.Pow(Constants.InchesPerFoot, 6);
            if (unit == LengthToThe6thUnit.MillimetersToThe6th) return value / Math.Pow(Constants.MillimetersPerMeter, 6) * Math.Pow(Constants.InchesPerMeter, 6);
            if (unit == LengthToThe6thUnit.MetersToThe6th) return value * Math.Pow(Constants.InchesPerMeter, 6);
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToLaTexString(ConvertTo(DisplayUnit), DisplayUnit);
        }
    }
}
