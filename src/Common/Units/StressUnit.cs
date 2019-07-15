using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class StressUnit : Unit
    {
        public static readonly StressUnit psi = new StressUnit(0, "psi");
        public static readonly StressUnit ksi = new StressUnit(1, "ksi");
        public static readonly StressUnit psf = new StressUnit(2, "psf");
        public static readonly StressUnit ksf = new StressUnit(3, "ksf");
        public static readonly StressUnit kPa = new StressUnit(4, "kPa");
        public static readonly StressUnit MPa = new StressUnit(5, "MPa");

        [JsonConstructor]
        private StressUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}