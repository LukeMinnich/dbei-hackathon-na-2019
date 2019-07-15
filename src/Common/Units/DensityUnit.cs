using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class DensityUnit : Unit
    {
        public static readonly DensityUnit PoundPerCubicInch = new DensityUnit(0, "lb/in^3");
        public static readonly DensityUnit PoundPerCubicFoot = new DensityUnit(1, "lb/ft^3");
        public static readonly DensityUnit KipPerCubicInch = new DensityUnit(2, "kip/in^3");
        public static readonly DensityUnit KipPerCubicFoot = new DensityUnit(3, "kip/ft^3");
        public static readonly DensityUnit KilogramPerCubicMillimeter = new DensityUnit(4, "kg/mm^3");
        public static readonly DensityUnit KilogramPerCubicMeter = new DensityUnit(5, "kg/m^3");

        [JsonConstructor]
        private DensityUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}