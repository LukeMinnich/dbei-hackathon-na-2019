using Kataclysm.Common.Extensions;
using Katerra.Apollo.Structures.Common.Units;

namespace Kataclysm.Common.Units.Conversion
{
    public static class Utilities
    {
        public static string ToLaTexString(double value, Unit unit)
        {
            return $@"{value.RoundToSignificantDigits(ProjectSettings.DEFAULT_SIGNIFICANT_DIGITS)} \text{{ }} {unit.Description}";
        }
        
        public static string ToUnitlessLaTexString(double value)
        {
            return $@"{value.RoundToSignificantDigits(ProjectSettings.DEFAULT_SIGNIFICANT_DIGITS)}";
        }
    }
}