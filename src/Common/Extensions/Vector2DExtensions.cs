using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Kataclysm.Common.Extensions
{
    public static class Vector2DExtensions
    {
        public static Point2D ToPoint2D(this Vector2D vector)
        {
            return new Point2D(vector.X, vector.Y);
        }

        public static Angle CounterClockwiseRotationToXAxis(this Vector2D vector)
        {
            var rotation = vector.AngleTo(Vector2D.XAxis);

            return (vector.Y > 0d)
                ? -1 * rotation
                : rotation;
        }
    }
}