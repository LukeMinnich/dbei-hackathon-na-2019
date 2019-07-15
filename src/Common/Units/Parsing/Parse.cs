using System;
using Kataclysm.Common.Units.Conversion;

namespace Katerra.Apollo.Structures.Common.Units.Parsing
{
    public static class Parse
    {
        public static Length FromFractionalInch(string designation)
        {
            if (designation == "") throw new ArgumentException("string cannot be empty");
            
            var quotesRemoved = designation.Replace("\"", "");
            var split = quotesRemoved.Split('/');
            
            if (split.Length > 2) throw new ArgumentException("could not convert string to fractional inch");

            return split.Length == 1
                ? new Length(double.Parse(split[0]), LengthUnit.Inch)
                : new Length(double.Parse(split[0]) / double.Parse(split[1]), LengthUnit.Inch);
        }
    }
}