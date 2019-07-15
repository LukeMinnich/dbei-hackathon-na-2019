using System;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Units.Conversion
{
    public class Undefined : Result
    {
        [JsonConstructor]
        private Undefined(FLT flt, double value, UndefinedUnit unit)
            : base(flt, value, unit)
        {
        }
        
        public Undefined(FLT flt, double value)
            : base(flt, NormalizeValue(value, UndefinedUnit.Undefined), DefaultUnit)
        {
        }

        public static UndefinedUnit DefaultUnit => UndefinedUnit.Undefined;

        public override double ConvertTo(Unit targetUnit)
        {
            if (targetUnit == UndefinedUnit.Undefined) return Value;
            
            throw new ArgumentException("cannot convert to the target unit");
        }

        protected static double NormalizeValue(double value, Unit unit)
        {
            if (unit == UndefinedUnit.Undefined) return value;
            
            throw new ArgumentException("cannot convert from the source unit");
        }

        public override string ToLaTexString()
        {
            return Utilities.ToUnitlessLaTexString(ConvertTo(DisplayUnit));
        }
    }
}