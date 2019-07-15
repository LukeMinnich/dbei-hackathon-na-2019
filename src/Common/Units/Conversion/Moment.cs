using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class Moment : Result
    {
        [JsonConstructor]
        private Moment(FLT flt, double value, MomentUnit unit)
            : base(flt, value, unit)
        {
        }
        
        public Moment(double value, MomentUnit unit)
            : base(FLT.Moment, NormalizeValue(value, unit), DefaultUnit)
        {
        }

        public static MomentUnit DefaultUnit => MomentUnit.KipInch;

        public static Result CreateWithStandardUnits(double value)
        {
            return new Moment(value, DefaultUnit);
        }
        
        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == MomentUnit.PoundInch) return Value * Constants.PoundsPerKip;
            if (targetUnit == MomentUnit.PoundFoot) return Value * Constants.PoundsPerKip / Constants.InchesPerFoot;
            if (targetUnit == MomentUnit.KipInch) return Value;
            if (targetUnit == MomentUnit.KipFoot) return Value / Constants.InchesPerFoot;
            if (targetUnit == MomentUnit.NewtonMeter) return Value / Constants.KipsPerKilonewton * Constants.NewtonsPerKilonewton / Constants.InchesPerMeter;
            if (targetUnit == MomentUnit.KilonewtonMeter) return Value / Constants.KipsPerKilonewton / Constants.InchesPerMeter;
            
            throw new ArgumentException("cannot convert to the target unit");
        }

        // Normalized to k-in
        protected static double NormalizeValue(double value, Unit unit)
        {
            if (unit == MomentUnit.PoundInch) return value / Constants.PoundsPerKip;
            if (unit == MomentUnit.PoundFoot) return value / Constants.PoundsPerKip * Constants.InchesPerFoot;
            if (unit == MomentUnit.KipInch) return value;
            if (unit == MomentUnit.KipFoot) return value * Constants.InchesPerFoot;
            if (unit == MomentUnit.NewtonMeter) return value / Constants.NewtonsPerKilonewton * Constants.KipsPerKilonewton * Constants.InchesPerMeter;
            if (unit == MomentUnit.KilonewtonMeter) return value * Constants.KipsPerKilonewton * Constants.InchesPerMeter;
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToLaTexString(ConvertTo(DisplayUnit), DisplayUnit);
        }
    }
}