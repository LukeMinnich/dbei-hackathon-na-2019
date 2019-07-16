using Kataclysm.Common;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class BearingWall : IReportsLevel
    {
        public string UniqueId { get; set; }
        public Point3D EndI { get; set; }
        public Point3D EndJ { get; set; }
        public BuildingLevel TopLevel { get; set; }
        public bool IsShearWall { get; set; } = true;
        public bool HasOpening { get; set; } = false;

        public BuildingLevel Level => TopLevel;

        public BearingWall()
        {

        }

        public BearingWall(string Id, Line2D wallLine, BuildingLevel level)
        {
            UniqueId = Id;
            EndI = new Point3D(wallLine.StartPoint.X, wallLine.StartPoint.Y, level.Elevation);
            EndJ = new Point3D(wallLine.EndPoint.X, wallLine.EndPoint.Y, level.Elevation);
            TopLevel = level;
        }
    }
}