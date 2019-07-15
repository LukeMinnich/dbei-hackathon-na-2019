using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class Stress : Result
    {
        [JsonConstructor]
        private Stress(FLT flt, double value, StressUnit unit)
            : base(flt, value, unit)
        {
        }
        
        public Stress(double value, StressUnit unit)
            : base(FLT.Stress, NormalizeValue(value, unit), DefaultUnit)
        {
        }

        public static StressUnit DefaultUnit => StressUnit.ksi;

        public static Result CreateWithStandardUnits(double value)
        {
            return new Stress(value, DefaultUnit);
        }

        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == StressUnit.psi) return Value * Constants.PoundsPerKip;
            if (targetUnit == StressUnit.ksi) return Value;
            if (targetUnit == StressUnit.psf) return Value * Constants.PoundsPerKip * (Constants.InchesPerFoot * Constants.InchesPerFoot);
            if (targetUnit == StressUnit.ksf) return Value * (Constants.InchesPerFoot * Constants.InchesPerFoot);
            if (targetUnit == StressUnit.kPa) return Value * (Constants.InchesPerMeter * Constants.InchesPerMeter) / Constants.KipsPerKilonewton;
            if (targetUnit == StressUnit.MPa) return Value * (Constants.InchesPerMeter * Constants.InchesPerMeter) / Constants.KipsPerKilonewton / Constants.KilonewtonPerMeganewton;
            
            throw new ArgumentException("cannot convert to the target unit");
        }

        // Normalized to ksi
        protected static double NormalizeValue(double value, Unit unit)
        {
            if (unit == StressUnit.psi) return value / Constants.PoundsPerKip;
            if (unit == StressUnit.ksi) return value;
            if (unit == StressUnit.psf) return value / Constants.PoundsPerKip / (Constants.InchesPerFoot * Constants.InchesPerFoot);
            if (unit == StressUnit.ksf) return value / (Constants.InchesPerFoot * Constants.InchesPerFoot);
            if (unit == StressUnit.kPa) return value / (Constants.InchesPerMeter * Constants.InchesPerMeter) * Constants.KipsPerKilonewton;
            if (unit == StressUnit.MPa) return value / (Constants.InchesPerMeter * Constants.InchesPerMeter) * Constants.KilonewtonPerMeganewton * Constants.KipsPerKilonewton;
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToLaTexString(ConvertTo((StressUnit) DisplayUnit), DisplayUnit);
        }
    }
}