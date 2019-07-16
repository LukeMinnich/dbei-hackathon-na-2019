using Kataclysm.Common;
using Kataclysm.Common.Units.Conversion;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class VerticalDistributionFactors : IReportsLevel
    {
        public BuildingLevel Level { get; set; }
        
        public Unitless X { get; set; }
        public Unitless Y { get; set; }
    }
}