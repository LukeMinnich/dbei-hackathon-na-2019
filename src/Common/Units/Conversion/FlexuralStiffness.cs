using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class FlexuralStiffness : Result
    {
        [JsonConstructor]
        private FlexuralStiffness(FLT flt, double value, FlexuralStiffnessUnit unit)
            : base(flt, value, unit)
        {
        }
        
        public FlexuralStiffness(double value, FlexuralStiffnessUnit unit)
            : base(FLT.FlexuralStiffness, NormalizeValue(value, unit), DefaultUnit)
        {
        }

        public static FlexuralStiffnessUnit DefaultUnit => FlexuralStiffnessUnit.KipInchSquared;
        
        public static Result CreateWithStandardUnits(double value)
        {
            return new FlexuralStiffness(value, DefaultUnit);
        }

        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == FlexuralStiffnessUnit.PoundInchSquared) return Value * Constants.PoundsPerKip;
            if (targetUnit == FlexuralStiffnessUnit.PoundFootSquared) return Value * Constants.PoundsPerKip / Math.Pow(Constants.InchesPerFoot,2);
            if (targetUnit == FlexuralStiffnessUnit.KipInchSquared) return Value;
            if (targetUnit == FlexuralStiffnessUnit.KipFootSquared) return Value / Math.Pow(Constants.InchesPerFoot,2);
            if (targetUnit == FlexuralStiffnessUnit.NewtonMeterSquared) return Value / Constants.KipsPerKilonewton * Constants.NewtonsPerKilonewton / Math.Pow(Constants.InchesPerMeter,2);
            if (targetUnit == FlexuralStiffnessUnit.KilonewtonMeterSquared) return Value / Constants.KipsPerKilonewton / Math.Pow(Constants.InchesPerMeter,2);
            
            throw new ArgumentException("cannot convert to the target unit");
        }

        protected static double NormalizeValue(double value, Unit unit)
        {
            if (unit == FlexuralStiffnessUnit.PoundInchSquared) return value / Constants.PoundsPerKip;
            if (unit == FlexuralStiffnessUnit.PoundFootSquared) return value / Constants.PoundsPerKip * Math.Pow(Constants.InchesPerFoot,2);
            if (unit == FlexuralStiffnessUnit.KipInchSquared) return value;
            if (unit == FlexuralStiffnessUnit.KipFootSquared) return value * Math.Pow(Constants.InchesPerFoot,2);
            if (unit == FlexuralStiffnessUnit.NewtonMeterSquared) return value / Constants.NewtonsPerKilonewton * Constants.KipsPerKilonewton * Math.Pow(Constants.InchesPerMeter,2);
            if (unit == FlexuralStiffnessUnit.KilonewtonMeterSquared) return value * Constants.KipsPerKilonewton * Math.Pow(Constants.InchesPerMeter,2);
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToLaTexString(ConvertTo(DisplayUnit), DisplayUnit);
        }
    }
}