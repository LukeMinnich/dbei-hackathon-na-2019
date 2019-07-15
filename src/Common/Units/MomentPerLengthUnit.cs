using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class MomentPerLengthUnit : Unit
    {
        public static readonly MomentPerLengthUnit PoundInchPerInch = new MomentPerLengthUnit(0, @"\frac{lb \cdot in}{in}");
        public static readonly MomentPerLengthUnit PoundFootPerFoot = new MomentPerLengthUnit(1, @"\frac{lb \cdot ft}{ft}");
        public static readonly MomentPerLengthUnit KipInchPerInch = new MomentPerLengthUnit(2, @"\frac{k \cdot in}{in}");
        public static readonly MomentPerLengthUnit KipFootPerFoot = new MomentPerLengthUnit(3, @"\frac{k \cdot ft}{ft}");
        public static readonly MomentPerLengthUnit NewtonMeterPerMeter = new MomentPerLengthUnit(4, @"\frac{N \cdot m}{m}");
        public static readonly MomentPerLengthUnit KilonewtonMeterPerMeter = new MomentPerLengthUnit(5, @"\frac{kN \cdot m}{m}");

        [JsonConstructor]
        private MomentPerLengthUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}