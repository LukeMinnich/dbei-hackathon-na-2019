using Kataclysm.Common;
using Kataclysm.Common.Geometry;
using Kataclysm.Common.Units.Conversion;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class OneWayDeck
    {
        public Polygon3D Boundary { get; set; }
        public Stress WeightPerArea { get; set; }
        public BuildingLevel Level { get; set; }
    }
}