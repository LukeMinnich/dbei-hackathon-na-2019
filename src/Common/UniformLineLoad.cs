using Kataclysm.Common.Extensions;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.Common
{
    public class UniformLineLoad : LineLoad
    {

        private double? _magnitude;

        public double? Magnitude
        {
            get
            {
                return _magnitude;
            }
            set
            {
                _magnitude = value;
                MagnitudeStart = value.Value;
                MagnitudeEnd = value.Value;
            }
        }

        public UniformLineLoad(LineSegment2D line, double magnitude, BuildingLevel level, Projection projection, LoadPattern loadPattern) : base(line, magnitude, magnitude, level, projection, loadPattern)
        {

        }

        public UniformLineLoad(LineSegment2D line, double magnitude) : base(line, magnitude, magnitude)
        {
        }

        public UniformLineLoad()
        {
        }

        public override Point2D LoadCentroid()
        {
            return (Line.StartPoint + 0.5 * Line.ToVector2D());
        }

        public override double LoadMagnitude()
        {
            return Line.Length * Magnitude.Value;
        }
    }
}