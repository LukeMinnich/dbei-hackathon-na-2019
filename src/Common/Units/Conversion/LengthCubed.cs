using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class LengthCubed : Result
    {
        [JsonConstructor]
        private LengthCubed(FLT flt, double value, LengthCubedUnit unit)
            : base(flt, value, unit)
        {
        }
        
        public LengthCubed(double value, LengthCubedUnit unit)
            : base(FLT.LengthCubed, NormalizeValue(value, unit), DefaultUnit)
        {
        }

        public static LengthCubedUnit DefaultUnit => LengthCubedUnit.InchesCubed;

        public static Result CreateWithStandardUnits(double value)
        {
            return new LengthCubed(value, DefaultUnit);
        }

        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == LengthCubedUnit.InchesCubed) return Value;
            if (targetUnit == LengthCubedUnit.FeetCubed) return Value / Math.Pow(Constants.InchesPerFoot, 3);
            if (targetUnit == LengthCubedUnit.MillimetersCubed) return Value / Math.Pow(Constants.InchesPerMeter, 3) * Math.Pow(Constants.MillimetersPerMeter, 3);
            if (targetUnit == LengthCubedUnit.MetersCubed) return Value / Math.Pow(Constants.InchesPerMeter, 3);
            
            throw new ArgumentException("cannot convert to the target unit");
        }

        protected static double NormalizeValue(double value, Unit unit)
        {
            if (unit == LengthCubedUnit.InchesCubed) return value;
            if (unit == LengthCubedUnit.FeetCubed) return value * Math.Pow(Constants.InchesPerFoot, 3);
            if (unit == LengthCubedUnit.MillimetersCubed) return value / Math.Pow(Constants.MillimetersPerMeter, 3) * Math.Pow(Constants.InchesPerMeter, 3);
            if (unit == LengthCubedUnit.MetersCubed) return value * Math.Pow(Constants.InchesPerMeter, 3);
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToLaTexString(ConvertTo(DisplayUnit), DisplayUnit);
        }
    }
}