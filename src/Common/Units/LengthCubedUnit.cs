using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class LengthCubedUnit : Unit
    {
        public static readonly LengthCubedUnit InchesCubed = new LengthCubedUnit(0, "in^3");
        public static readonly LengthCubedUnit FeetCubed = new LengthCubedUnit(1, "ft^3");
        public static readonly LengthCubedUnit MillimetersCubed = new LengthCubedUnit(2, "mm^3");
        public static readonly LengthCubedUnit MetersCubed = new LengthCubedUnit(3, "m^3");

        [JsonConstructor]
        private LengthCubedUnit(int value, string description)
            : base(value, description)
        {
        }
    } 
}