using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class FlexuralStiffnessUnit : Unit
    {
        public static readonly FlexuralStiffnessUnit PoundInchSquared = new FlexuralStiffnessUnit(0, @"lb \cdot in^2");
        public static readonly FlexuralStiffnessUnit PoundFootSquared = new FlexuralStiffnessUnit(1, @"lb \cdot ft^2");
        public static readonly FlexuralStiffnessUnit KipInchSquared = new FlexuralStiffnessUnit(2, @"k \cdot in^2");
        public static readonly FlexuralStiffnessUnit KipFootSquared = new FlexuralStiffnessUnit(3, @"k \cdot ft^2");
        public static readonly FlexuralStiffnessUnit NewtonMeterSquared = new FlexuralStiffnessUnit(4, @"N \cdot m^2");
        public static readonly FlexuralStiffnessUnit KilonewtonMeterSquared = new FlexuralStiffnessUnit(5, @"kN \cdot m^2");

        [JsonConstructor]
        private FlexuralStiffnessUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}