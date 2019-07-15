using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Kataclysm.Common.Extensions
{
    public static class Point3DExtensions
    {
        public static Point2D ToPoint2D(this Point3D point)
        {
            return new Point2D(point.X, point.Y);
        }

        public static Point2D ToPoint2D(this Point3D? point)
        {
            return new Point2D(((Point3D)point).X, ((Point3D)point).Y);
        }

        public static Point3D RotateAroundZAxis(this Point3D point, Angle angle, Point2D center)
        {
            var shiftVector = new Point3D(center.X, center.Y, 0).VectorTo(Point3D.Origin);

            var tempPoint = point + shiftVector;
                
            tempPoint = tempPoint.Rotate(UnitVector3D.ZAxis, angle);

            return tempPoint - shiftVector;
        }
    }
}