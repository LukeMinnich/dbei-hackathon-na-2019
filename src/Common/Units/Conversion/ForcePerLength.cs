using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class ForcePerLength : Result
    {
        [JsonConstructor]
        private ForcePerLength(FLT flt, double value, ForcePerLengthUnit unit)
            : base(flt, value, unit)
        {
        }
        
        public ForcePerLength(double value, ForcePerLengthUnit unit)
            : base(FLT.ForcePerLength, NormalizeValue(value, unit), DefaultUnit)
        {
        }

        public static ForcePerLengthUnit DefaultUnit => ForcePerLengthUnit.KipPerInch;
        
        public static Result CreateWithStandardUnits(double value)
        {
            return new ForcePerLength(value, DefaultUnit);
        }

        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == ForcePerLengthUnit.PoundPerInch) return Value * Constants.PoundsPerKip;
            if (targetUnit == ForcePerLengthUnit.PoundPerFoot) return Value * Constants.PoundsPerKip * Constants.InchesPerFoot;
            if (targetUnit == ForcePerLengthUnit.KipPerInch) return Value;
            if (targetUnit == ForcePerLengthUnit.KipPerFoot) return Value * Constants.InchesPerFoot;
            if (targetUnit == ForcePerLengthUnit.NewtonPerMeter) return Value / Constants.KipsPerKilonewton * Constants.NewtonsPerKilonewton * Constants.InchesPerMeter;
            if (targetUnit == ForcePerLengthUnit.KilonewtonPerMeter) return Value / Constants.KipsPerKilonewton * Constants.InchesPerMeter;
            
            throw new ArgumentException("cannot convert to the target unit");
        }

        protected static double NormalizeValue(double value, Unit unit)
        {
            if (unit == ForcePerLengthUnit.PoundPerInch) return value / Constants.PoundsPerKip;
            if (unit == ForcePerLengthUnit.PoundPerFoot) return value / Constants.PoundsPerKip / Constants.InchesPerFoot;
            if (unit == ForcePerLengthUnit.KipPerInch) return value;
            if (unit == ForcePerLengthUnit.KipPerFoot) return value / Constants.InchesPerFoot;
            if (unit == ForcePerLengthUnit.NewtonPerMeter) return value / Constants.NewtonsPerKilonewton * Constants.KipsPerKilonewton / Constants.InchesPerMeter;
            if (unit == ForcePerLengthUnit.KilonewtonPerMeter) return value * Constants.KipsPerKilonewton / Constants.InchesPerMeter;
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToLaTexString(ConvertTo(DisplayUnit), DisplayUnit);
        }
    }
}