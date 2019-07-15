using System;

namespace Kataclysm.Common.Units.Conversion
{
    public static class Constants
    {
        // Within imperial system
        public const int InchesPerFoot = 12;
        public const int PoundsPerKip = 1000;
        
        // Within metric system
        public const int MillimetersPerMeter = 1000;
        public const int NewtonsPerKilonewton = 1000;
        public const int KilonewtonPerMeganewton = 1000;
        
        // Between imperial and metric
        public const double InchesPerMeter = 39.37007874;
        public const double KipsPerKilonewton = 0.2248089438;
        public const double KipsPerKilogram = 0.00045359237;
        
        // Other
        public const double DegreesPerRadian = 180 / Math.PI;
        public static readonly double SpecificGravityPerDensity = 1000d * Math.Pow(12d, 3) / 62.4;
    }
}