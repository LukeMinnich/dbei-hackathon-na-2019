using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class LengthToThe6thUnit : Unit
    {
        public static readonly LengthToThe6thUnit InchesToThe6th = new LengthToThe6thUnit(0, "in^6");
        public static readonly LengthToThe6thUnit FeetToThe6th = new LengthToThe6thUnit(1, "ft^6");
        public static readonly LengthToThe6thUnit MillimetersToThe6th = new LengthToThe6thUnit(2, "mm^6");
        public static readonly LengthToThe6thUnit MetersToThe6th = new LengthToThe6thUnit(3, "m^6");

        [JsonConstructor]
        private LengthToThe6thUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}
