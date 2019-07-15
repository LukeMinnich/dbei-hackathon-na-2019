using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class LengthUnit : Unit
    {
        public static readonly LengthUnit Inch = new LengthUnit(0, "in");
        public static readonly LengthUnit Foot = new LengthUnit(1, "ft");
        public static readonly LengthUnit Millimeter = new LengthUnit(2, "mm");
        public static readonly LengthUnit Meter = new LengthUnit(3, "m");

        [JsonConstructor]
        private LengthUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}