using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using Kataclysm.Common.Units.Conversion;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class AnalyticalWallLateral
    {
        public LineSegment2D WallLine { get; set; }
        public string UniqueId { get; set; }
        public ForcePerLength Stiffness { get; set; }
        public BuildingLevel TopLevel { get; set; }

        [JsonIgnore] public Point2D Centroid => WallLine.Midpoint();

        public AnalyticalWallLateral(string id, ForcePerLength stiffness, Point2D endI, Point2D endJ)
        {
            WallLine = new LineSegment2D(endI, endJ);
            UniqueId = id;
            Stiffness = stiffness;
        }
    }
}