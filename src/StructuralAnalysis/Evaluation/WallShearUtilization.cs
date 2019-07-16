using System;
using Kataclysm.Common.Units.Conversion;
using Katerra.Apollo.Structures.Common.Units;

namespace Kataclysm.StructuralAnalysis.Evaluation
{
    public class WallShearUtilization
    {
        public ForcePerLength UnitShear { get; set; }
        
        private static ForcePerLength NormalizeShearTo { get; } = new ForcePerLength(1000, ForcePerLengthUnit.PoundPerFoot);
        private static ForcePerLength UpperLimitOnShear { get; } = new ForcePerLength(2000, ForcePerLengthUnit.PoundPerFoot);

        private static double costAtZeroShear = 0.5;
        private static double costAtNormalizedShear = 1.0;
        private static double costAtUpperShearLimit = 3.0;

        public bool ExceedsLimit => UnitShear >= UpperLimitOnShear;
        private double UpperLimitShearRatio => (UpperLimitOnShear / NormalizeShearTo).ConvertTo(UnitlessUnit.Unitless);
        
        // Up to 1000 plf

        public WallShearUtilization(ForcePerLength unitShear)
        {
            UnitShear = unitShear;
        }

        public double Normalize()
        {
            return (UnitShear / NormalizeShearTo).ConvertTo(UnitlessUnit.Unitless);
        }

        public double GetNormalizedCost()
        {
            var normalizedShear = Normalize();

            return CostModel(normalizedShear);
        }

        private double CostModel(double normalizedShear)
        {
            // Linear function below 1.0
            if (normalizedShear < 1.0)
            {
                double m = (costAtNormalizedShear - costAtZeroShear) / (1.0 - 0);
                double x = normalizedShear;
                double b = costAtZeroShear;

                return m * x + b;
            }

            // Quadratic function above 1.0, vertex at normalized value
            else
            {
                double h = 1.0;
                double k = costAtNormalizedShear;

                double a = (costAtUpperShearLimit - k) / Math.Pow(UpperLimitShearRatio - h, 2);

                return a * Math.Pow(normalizedShear - h, 2) + k;
            }
        }
    }
}