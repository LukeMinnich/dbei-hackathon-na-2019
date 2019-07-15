using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class LengthToThe4th : Result
    {
        [JsonConstructor]
        private LengthToThe4th(FLT flt, double value, LengthToThe4thUnit unit)
            : base(flt, value, unit)
        {
        }
        
        public LengthToThe4th(double value, LengthToThe4thUnit unit)
            : base(FLT.LengthToThe4th, NormalizeValue(value, unit), DefaultUnit)
        {
        }

        public static LengthToThe4thUnit DefaultUnit => LengthToThe4thUnit.InchesToThe4th;

        public static Result CreateWithStandardUnits(double value)
        {
            return new LengthToThe4th(value, DefaultUnit);
        }

        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == LengthToThe4thUnit.InchesToThe4th) return Value;
            if (targetUnit == LengthToThe4thUnit.FeetToThe4th) return Value / Math.Pow(Constants.InchesPerFoot, 4);
            if (targetUnit == LengthToThe4thUnit.MillimetersToThe4th) return Value / Math.Pow(Constants.InchesPerMeter, 4) * Math.Pow(Constants.MillimetersPerMeter, 4);
            if (targetUnit == LengthToThe4thUnit.MetersToThe4th) return Value / Math.Pow(Constants.InchesPerMeter, 4);
            
            throw new ArgumentException("cannot convert to the target unit");
        }

        protected static double NormalizeValue(double value, Unit unit)
        {
            if (unit == LengthToThe4thUnit.InchesToThe4th) return value;
            if (unit == LengthToThe4thUnit.FeetToThe4th) return value * Math.Pow(Constants.InchesPerFoot, 4);
            if (unit == LengthToThe4thUnit.MillimetersToThe4th) return value / Math.Pow(Constants.MillimetersPerMeter, 4) * Math.Pow(Constants.InchesPerMeter, 4);
            if (unit == LengthToThe4thUnit.MetersToThe4th) return value * Math.Pow(Constants.InchesPerMeter, 4);
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToLaTexString(ConvertTo(DisplayUnit), DisplayUnit);
        }
    }
}