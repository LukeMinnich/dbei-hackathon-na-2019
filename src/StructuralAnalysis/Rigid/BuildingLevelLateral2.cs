using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using Kataclysm.Common.Geometry;
using Kataclysm.Common.Units.Conversion;
using Kataclysm.StructuralAnalysis.Model;
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

        public BuildingLevelLateral2(OneWayDeck deck)
        {
            Level = deck.Level;
            Boundary = deck.Boundary;
            CenterOfMass = deck.Boundary.GetCentroid();
            SeismicWeight = deck.SeismicWeight;

            var bounds = Boundary.GetMinMaxBounds();
            LengthX = new Length(bounds.MaxX - bounds.MinX, LengthUnit.Inch);
            LengthY = new Length(bounds.MaxY - bounds.MinY, LengthUnit.Inch);
        }
    }
}