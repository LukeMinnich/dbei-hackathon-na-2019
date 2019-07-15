using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class MomentUnit : Unit
    {
        public static readonly MomentUnit PoundInch = new MomentUnit(0, @"lb \cdot in");
        public static readonly MomentUnit PoundFoot = new MomentUnit(1, @"lb \cdot ft");
        public static readonly MomentUnit KipInch = new MomentUnit(2, @"k \cdot in");
        public static readonly MomentUnit KipFoot = new MomentUnit(3, @"k \cdot ft");
        public static readonly MomentUnit NewtonMeter = new MomentUnit(4, @"N \cdot m");
        public static readonly MomentUnit KilonewtonMeter = new MomentUnit(5, @"kN \cdot m");

        [JsonConstructor]
        private MomentUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}