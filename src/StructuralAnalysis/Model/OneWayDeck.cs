using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using Kataclysm.Common.Units.Conversion;
using Katerra.Apollo.Structures.Common.Units;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class OneWayDeck
    {
        public Polygon2D Boundary { get; set; }
        public Stress WeightPerArea { get; set; }
        public BuildingLevel Level { get; set; }
        
        [JsonIgnore]
        public Force SeismicWeight
        {
            get
            {
                var area = new Area(Boundary.GetArea(), AreaUnit.SquareInch);
                return (Force) (WeightPerArea * area);
            }
        }
    }
}