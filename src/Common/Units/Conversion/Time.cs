using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class Time : Result
    {
        [JsonConstructor]
        private Time(FLT flt, double value, TimeUnit unit)
            : base(flt, value, unit)
        {
        }
        
        // Always in seconds
        public Time(double value)
            : base(FLT.Time, NormalizeValue(value, TimeUnit.Seconds), DefaultUnit)
        {
        }

        public static TimeUnit DefaultUnit => TimeUnit.Seconds;

        public static Result CreateWithStandardUnits(double value)
        {
            return new Time(value);
        }

        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == TimeUnit.Seconds) return Value;
            
            throw new ArgumentException("cannot convert to the target unit");
        }

        protected static double NormalizeValue(double value, Unit unit)
        {
            if (unit == TimeUnit.Seconds) return value;
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToLaTexString(ConvertTo(DisplayUnit), DisplayUnit);
        }
    }
}