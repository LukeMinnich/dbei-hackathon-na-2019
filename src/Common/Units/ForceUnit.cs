using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class ForceUnit : Unit
    {
        public static readonly ForceUnit Pound = new ForceUnit(0, "lb");
        public static readonly ForceUnit Kip = new ForceUnit(1, "kip");
        public static readonly ForceUnit Newton = new ForceUnit(2, "N");
        public static readonly ForceUnit Kilonewton = new ForceUnit(3, "kN");
        
        [JsonConstructor]
        private ForceUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}