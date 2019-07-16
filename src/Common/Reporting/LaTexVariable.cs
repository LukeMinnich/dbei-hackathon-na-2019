using Kataclysm.Common.Units.Conversion;
using Katerra.Apollo.Structures.Common.Units;

namespace Kataclysm.Common.Reporting
{
    public class LaTexVariable
    {
        public string LaTexMoniker { get; }
        public Result Result { get; }

        public LaTexVariable(string laTexMoniker, double value)
            : this(laTexMoniker, new Unitless(value), UnitlessUnit.Unitless)
        {
        }
        
        public LaTexVariable(string laTexMoniker, Result result, Unit displayUnit)
        {
            LaTexMoniker = laTexMoniker;
            Result = 1 * new Undefined(result.FLT, result.Value); // Forces cast of underlying Result type to the target subtype
            Result.DisplayUnit = displayUnit;
        }

        public string ToInlineString()
        {
            return Result.ToLaTexString();
        }
    }
}