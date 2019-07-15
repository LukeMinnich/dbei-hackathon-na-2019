using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class AreaUnit : Unit
    {
        public static readonly AreaUnit SquareInch = new AreaUnit(0, @"in^2");
        public static readonly AreaUnit SquareFoot = new AreaUnit(1, @"ft^2");
        public static readonly AreaUnit SquareMillimeter = new AreaUnit(2, @"mm^2");
        public static readonly AreaUnit SquareMeter = new AreaUnit(3, @"m^2");

        [JsonConstructor]
        private AreaUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}