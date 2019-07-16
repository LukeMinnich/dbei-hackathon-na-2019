using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class  PointLoad : FloorLoad
    {
        public Point2D? Location { get; set; }
        public double? Magnitude { get; set; }

        public PointLoad(Point2D location, double magnitude, BuildingLevel level, Projection projection, LoadPattern loadPattern) : base(level, projection, loadPattern)
        {
            Location = location;
            Magnitude = magnitude;
        }

        public PointLoad(Point2D location, double magnitude) : base()
        {
            Location = location;
            Magnitude = magnitude;
        }

        public PointLoad()
        {
        }
    }
}