using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using Kataclysm.Common.Units.Conversion;
using Kataclysm.StructuralAnalysis.Model;
using Katerra.Apollo.Structures.Common.Units;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class AnalyticalWallLateral
    {
        public LineSegment2D WallLine { get; set; }
        public string UniqueId { get; set; }
        public BuildingLevel TopLevel { get; set; }

        [JsonIgnore] public Point2D Centroid => WallLine.Midpoint();
        [JsonIgnore] public ForcePerLength Stiffness => new ForcePerLength(WallLine.Length / 12, ForcePerLengthUnit.KipPerInch);

        public AnalyticalWallLateral(BearingWall wall)
            : this(wall.UniqueId, wall.EndI.ToPoint2D(), wall.EndJ.ToPoint2D(), wall.TopLevel)
        {
        }
        
        public AnalyticalWallLateral(string id, Point2D endI, Point2D endJ, BuildingLevel topLevel)
        {
            WallLine = new LineSegment2D(endI, endJ);
            UniqueId = id;
            TopLevel = topLevel;
        }
    }
}