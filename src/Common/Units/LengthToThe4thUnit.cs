using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class LengthToThe4thUnit : Unit
    {
        public static readonly LengthToThe4thUnit InchesToThe4th = new LengthToThe4thUnit(0, "in^4");
        public static readonly LengthToThe4thUnit FeetToThe4th = new LengthToThe4thUnit(1, "ft^4");
        public static readonly LengthToThe4thUnit MillimetersToThe4th = new LengthToThe4thUnit(2, "mm^4");
        public static readonly LengthToThe4thUnit MetersToThe4th = new LengthToThe4thUnit(3, "m^4");
        
        [JsonConstructor]
        private LengthToThe4thUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}