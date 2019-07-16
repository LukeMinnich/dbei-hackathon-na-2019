using Kataclysm.Common;
using Kataclysm.Common.Units.Conversion;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class SeismicStoryForce : IReportsLevel
    {
        public BuildingLevel Level { get; set; }
        public Force X { get; set; }
        public Force Y { get; set; }
    }
}