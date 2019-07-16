using System;

namespace Kataclysm.Common.Extensions
{ 
    public static class DoubleExtensions
    {
        private const double INTEGER_NEARNESS_TOLERANCE = Double.Epsilon * 100;
        
        public static bool IsNanOrInfinity(this double value)
        {
            return Double.IsNaN(value) || Double.IsInfinity(value);
        }
        
        public static bool ContainsNanOrInfinity(this double[,] value)
        {
            for (int i = 0; i <= value.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= value.GetUpperBound(1); j++)
                {
                    if (value[i,j].IsNanOrInfinity() == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static double Pow(this double x, double y)
        {
            return Math.Pow(x, y);
        }

        public static double ToRadians(this double value)
        {
            return value * 2.0 * Math.PI / 360.0;
        }

        public static bool IsBasicallyAnInteger(this double value, out int nearestInteger)
        {
            nearestInteger = (int) (value + INTEGER_NEARNESS_TOLERANCE);

            return IsBasicallyAnInteger(value);
        }

        public static bool IsBasicallyAnInteger(this double value)
        {
            return Math.Abs(value % 1) <= INTEGER_NEARNESS_TOLERANCE;
        }

        public static string ToStringWithTrailingZeros(this double value, int precision)
        {
            if (precision < 0) throw new ArgumentException("cannot format with fewer than zero decimal places");
            
            return Math.Round(value, precision).ToString($"N{precision}");
        }

        public static double AbsoluteValueEnvelope(this double value, double other)
        {
            return Math.Max(Math.Abs(value), Math.Abs(other));
        }
    }
}
