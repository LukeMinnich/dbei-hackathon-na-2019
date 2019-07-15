using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class ForcePerLengthUnit : Unit
    {
        public static readonly ForcePerLengthUnit PoundPerInch = new ForcePerLengthUnit(0, "lb/in");
        public static readonly ForcePerLengthUnit PoundPerFoot = new ForcePerLengthUnit(1, "lb/ft");
        public static readonly ForcePerLengthUnit KipPerInch = new ForcePerLengthUnit(2, "k/in");
        public static readonly ForcePerLengthUnit KipPerFoot = new ForcePerLengthUnit(3, "k/ft");
        public static readonly ForcePerLengthUnit NewtonPerMeter = new ForcePerLengthUnit(4, "N/m");
        public static readonly ForcePerLengthUnit KilonewtonPerMeter = new ForcePerLengthUnit(5, "kN/m");

        [JsonConstructor]
        private ForcePerLengthUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}