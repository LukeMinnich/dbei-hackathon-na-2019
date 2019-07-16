using Kataclysm.Common;
using Kataclysm.Common.Geometry;
using Kataclysm.Common.Units.Conversion;
using Katerra.Apollo.Structures.Common.Units;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class BuildingLevelLateral2 : IReportsLevel
    {
        public BuildingLevel Level { get; }
        public Polygon2D Boundary { get; }
        public Force SeismicWeight { get; }
        
        public Length LengthX { get; }
        public Length LengthY { get; }
        public Length Height { get; set; }
        
        public Point2D CenterOfMass { get; }
        public Point2D CenterOfRigidity { get; set; }
        public AccidentalEccentricities Eccentricities { get; set; }

        public BuildingLevelLateral2(BuildingLevel level, MassCenter massCharacteristics, Polygon2D boundary)
        {
            Level = level;
            Boundary = boundary;
            CenterOfMass = massCharacteristics.CenterOfMass;
            SeismicWeight = new Force(massCharacteristics.Weight, ForceUnit.Kip);

            var bounds = Boundary.GetMinMaxBounds();
            LengthX = new Length(bounds.MaxX - bounds.MinX, LengthUnit.Inch);
            LengthY = new Length(bounds.MaxY - bounds.MinY, LengthUnit.Inch);
        }
    }
}