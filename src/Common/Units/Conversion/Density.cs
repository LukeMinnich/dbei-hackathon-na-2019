using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class Density : Result
    {
        [JsonConstructor]
        private Density(FLT flt, double value, DensityUnit unit)
            : base(flt, value, unit)
        {
        }
        
        public Density(double value, DensityUnit unit)
            : base(FLT.Density, NormalizeValue(value, unit), DefaultUnit)
        {
        }

        public static DensityUnit DefaultUnit => DensityUnit.KipPerCubicInch;

        public static Result CreateWithStandardUnits(double value)
        {
            return new Density(value, DefaultUnit);
        }

        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == DensityUnit.PoundPerCubicInch) return Value * Constants.PoundsPerKip;
            if (targetUnit == DensityUnit.PoundPerCubicFoot) return Value * Constants.PoundsPerKip * Math.Pow(Constants.InchesPerFoot, 3);
            if (targetUnit == DensityUnit.KipPerCubicInch) return Value;
            if (targetUnit == DensityUnit.KipPerCubicFoot) return Value * Math.Pow(Constants.InchesPerFoot, 3);
            if (targetUnit == DensityUnit.KilogramPerCubicMillimeter) return Value / Constants.KipsPerKilogram * Math.Pow(Constants.InchesPerMeter, 3) / Math.Pow(Constants.MillimetersPerMeter, 3);
            if (targetUnit == DensityUnit.KilogramPerCubicMeter) return Value / Constants.KipsPerKilogram * Math.Pow(Constants.InchesPerMeter, 3);
            
            throw new ArgumentException("cannot convert to the target unit");
        }

        // Normalized to k/in^3
        protected static double NormalizeValue(double value, Unit unit)
        {
            if (unit == DensityUnit.PoundPerCubicInch) return value / Constants.PoundsPerKip;
            if (unit == DensityUnit.PoundPerCubicFoot) return value / Constants.PoundsPerKip / Math.Pow(Constants.InchesPerFoot, 3);
            if (unit == DensityUnit.KipPerCubicInch) return value;
            if (unit == DensityUnit.KipPerCubicFoot) return value / Math.Pow(Constants.InchesPerFoot, 3);
            if (unit == DensityUnit.KilogramPerCubicMillimeter) return value * Constants.KipsPerKilogram * Math.Pow(Constants.MillimetersPerMeter, 3) / Math.Pow(Constants.InchesPerMeter, 3);
            if (unit == DensityUnit.KilogramPerCubicMeter) return value * Constants.KipsPerKilogram / Math.Pow(Constants.InchesPerMeter, 3);
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToLaTexString(ConvertTo(DisplayUnit), DisplayUnit);
        }
    }
}