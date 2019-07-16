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
        public bool IsShearWall { get; set; }
        public bool HasOpening { get; set; } = false;

        public BuildingLevel Level => TopLevel;
    }
}