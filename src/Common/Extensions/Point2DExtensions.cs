using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Kataclysm.Common.Extensions
{
    public static class Point2DExtensions
    {
        public static Point3D MapToPlane(this Point2D point, Plane plane)
        {
            var a = plane.A;
            var b = plane.B;
            var c = plane.C;
            var d = plane.D;

            var x = point.X;
            var y = point.Y;

            var z = -1 * (a * x + b * y + d) / c;

            return new Point3D(x, y, z);
        }

        public static bool LiesOnPolyline(this Point2D point, PolyLine2D line, double Tolerance = 0.0001)
        {
            List<Point2D> Vertices = line.Vertices.ToList();

            for (int i = 0; i < Vertices.Count - 1; i++)
            {
                Point2D v0 = Vertices[i];
                Point2D v1 = Vertices[i + 1];

                double MaxX = Math.Max(v0.X, v1.X);
                double MinX = Math.Min(v0.X, v1.X);

                double MaxY = Math.Max(v0.Y, v1.Y);
                double MinY = Math.Min(v0.Y, v1.Y);

                if (v0.X == v1.X)
                {
                    if (Math.Abs(point.X - v0.X) <= Tolerance)
                    {
                        if (point.Y >= MinY - Tolerance && point.Y <= MaxY + Tolerance)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    double m = (v1.Y - v0.Y) / (v1.X - v0.X);
                    double b = v0.Y - m * v0.X;

                    if (point.X >= MinX - Tolerance && point.X <= MaxX + Tolerance)
                    {
                        if (Math.Abs(point.Y - (m * point.X + b)) <= Tolerance)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool LiesOnLine(this Point2D point, LineSegment2D line, double Tolerance = 0.0001)
        {
            List<Point2D> Vertices = new List<Point2D>();
            Vertices.Add(line.StartPoint);
            Vertices.Add(line.EndPoint);

            for (int i = 0; i < Vertices.Count - 1; i++)
            {
                Point2D v0 = Vertices[i];
                Point2D v1 = Vertices[i + 1];

                double MaxX = Math.Max(v0.X, v1.X);
                double MinX = Math.Min(v0.X, v1.X);

                double MaxY = Math.Max(v0.Y, v1.Y);
                double MinY = Math.Min(v0.Y, v1.Y);

                if (v0.X == v1.X)
                {
                    if (Math.Abs(point.X - v0.X) <= Tolerance)
                    {
                        if (point.Y >= MinY && point.Y <= MaxY)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    double m = (v1.Y - v0.Y) / (v1.X - v0.X);
                    double b = v0.Y - m * v0.X;

                    if (point.X >= MinX && point.X <= MaxX)
                    {
                        if (Math.Abs(point.Y - (m * point.X + b)) <= Tolerance)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // Per https://stackoverflow.com/questions/11907947/how-to-check-if-a-point-lies-on-a-line-between-2-other-points
        public static bool IsOn(this Point2D point, LineSegment2D segment)
        {
            Vector2D toPoint = segment.StartPoint.VectorTo(point);
            Vector2D toEnd = segment.ToVector2D();

            var cross = toPoint.CrossProduct(toEnd);

            // TODO Luke to determine best way to calculate collinearity tolerance

            if (Math.Abs(cross) > 1e-5) return false;

            if (Math.Abs(toEnd.X) >= Math.Abs(toEnd.Y))
            {
                return (toEnd.X > 0)
                    ? segment.StartPoint.X <= point.X && point.X <= segment.EndPoint.X
                    : segment.EndPoint.X <= point.X && point.X <= segment.StartPoint.X;
            }
            else
            {
                return (toEnd.Y > 0)
                    ? segment.StartPoint.Y <= point.Y && point.Y <= segment.EndPoint.Y
                    : segment.EndPoint.Y <= point.Y && point.Y <= segment.StartPoint.Y;
            }
        }

        public static bool IsOn(this Point2D point, Polygon2D polygon)
        {
            foreach (var edge in polygon.Edges)
            {
                if (point.IsOn(edge)) return true;
            }

            return false;
        }

        public static Point2D Rotate(this Point2D point, Angle angle)
        {
            var vector = point.ToVector2D();

            return vector.Rotate(angle).ToPoint2D();
        }

        public static double DistanceToLineExtension(this Point2D point, LineSegment2D segment)
        {
            var nearestPoint = segment.ClosestPointToExtension(point);

            return nearestPoint.DistanceTo(point);
        }

        // TODO Luke consider re-writing so that tolerance is an input argument
        public static List<Point2D> OrderCollinearPoints(this IEnumerable<Point2D> points)
        {
            var allPoints = points.ToList();

            var testX = allPoints[0].X;

            var query = allPoints.Where(p => Math.Abs(p.X - testX) < 1e-6).ToList();

            return (query.Count != allPoints.Count)
                ? allPoints.OrderBy(p => p.X).ToList()
                : allPoints.OrderBy(p => p.Y).ToList();
        }

        public static IEnumerable<PreVertex> ConvertToPreVertices(this IEnumerable<Point2D> points)
        {
            var convertedPoints = new List<PreVertex>();

            foreach (var point in points)
            {
                convertedPoints.Add(point.ConvertToPreVertex());
            }

            return convertedPoints;
        }

        public static PreVertex ConvertToPreVertex(this Point2D point)
        {
            return new PreVertex(point.X, point.Y);
        }

        public static IEnumerable<Point2D> ConvertFromPreVertices(IEnumerable<PreVertex> points)
        {
            var convertedPoints = new List<Point2D>();

            foreach (var point in points)
            {
                convertedPoints.Add(ConvertFromPreVertex(point));
            }

            return convertedPoints;
        }

        public static Point2D ConvertFromPreVertex(PreVertex point)
        {
            return new Point2D(point.X, point.Y);
        }

        public static LineSegment2D LongestSegment(this IEnumerable<Point2D> points)
        {
            var allPoints = points.ToList();

            if (allPoints.Count < 2) throw new ArgumentException("Cannot have fewer than 2 points.");

            var firstSegment = new LineSegment2D(allPoints[0], allPoints[1]);

            Tuple<double, LineSegment2D> longest = new Tuple<double, LineSegment2D>(firstSegment.Length, firstSegment);

            foreach (var point in allPoints)
            {
                foreach (var otherPoint in allPoints)
                {
                    if (point.DistanceTo(otherPoint) > longest.Item1)
                    {
                        var updateSegment = new LineSegment2D(point, otherPoint);
                        longest = new Tuple<double, LineSegment2D>(updateSegment.Length, updateSegment);
                    }
                }
            }

            return longest.Item2;
        }

        public static Point3D ToPoint3D(this Point2D point, double zDimension)
        {
            return new Point3D(point.X, point.Y, zDimension);
        }

        public static List<Point2D> DistinctBasedOnDistanceApart(this IEnumerable<Point2D> points,
            double epsilon)
        {
            var allPoints = new LinkedList<Point2D>(points);

            var item = allPoints.First;

            while (item != null)
            {
                var otherItem = allPoints.First;

                while (otherItem != null)
                {
                    var nextItem = otherItem.Next;

                    if (item != otherItem)
                    {
                        Point2D point = item.Value;
                        Point2D otherPoint = otherItem.Value;

                        if (point.DistanceTo(otherPoint) < epsilon) allPoints.Remove(otherItem);
                    }

                    otherItem = nextItem;
                }

                item = item.Next;
            }

            return allPoints.ToList();
        }

        public static List<Point2D> ReplaceBasedOnDistanceApart(this IEnumerable<Point2D> points,
            IEnumerable<Point2D> replacementPoints, double epsilon)
        {
            var allPoints = new LinkedList<Point2D>(points);

            var replacements = replacementPoints.ToList();

            var item = allPoints.First;

            while (item != null)
            {
                var point = item.Value;

                foreach (var replacement in replacements)
                {
                    if (point.DistanceTo(replacement) < epsilon) item.Value = replacement;
                }

                item = item.Next;
            }

            return allPoints.ToList();
        }

        public static double GetAngleTo(this Point2D point, Point2D nextPoint)
        {
            double angle = 0;

            if (nextPoint.Y > point.Y && nextPoint.X > point.X)
            {
                //Quadrant 1
                angle = Math.Atan((nextPoint.Y - point.Y) / (nextPoint.X - point.X));
            }
            else if (nextPoint.Y > point.Y && nextPoint.X < point.X)
            {
                //Quadrant 2
                angle = Math.PI - Math.Abs(Math.Atan((nextPoint.Y - point.Y) / (nextPoint.X - point.X)));
            }
            else if (nextPoint.Y < point.Y && nextPoint.X < point.X)
            {
                //Quadrant 3
                angle = Math.Abs(Math.Atan((nextPoint.Y - point.Y) / (nextPoint.X - point.X))) + Math.PI;
            }
            else if (nextPoint.Y < point.Y && nextPoint.X > point.X)
            {
                //Quadrant 4
                angle = 2d * Math.PI + Math.Atan((nextPoint.Y - point.Y) / (nextPoint.X - point.X));
            }
            else if (nextPoint.Y == point.Y && nextPoint.X > point.X)
            {
                angle = 0d;
            }
            else if (nextPoint.Y == point.Y && nextPoint.X < point.X)
            {
                angle = Math.PI;
            }
            else if (nextPoint.Y > point.Y && nextPoint.X == point.X)
            {
                angle = Math.PI / 2d;
            }
            else if (nextPoint.Y < point.Y && nextPoint.X == point.X)
            {
                angle = 3d * Math.PI / 2d;
            }

            return angle;
        }

        public static bool IsSamePointWithinTolerance(this Point2D v0, Point2D v1, double orthogonalTolerance)
        {
            if (v0.Equals(v1))
            {
                return true;
            }
            else
            {
                if (Math.Abs(v1.X - v0.X) <= orthogonalTolerance && Math.Abs(v1.Y - v0.Y) <= orthogonalTolerance)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsAtCornerWithinTolerance(this Point2D point, Polygon2D boundary, double orthogonalTolerance)
        {
            return boundary.Vertices.Any(other => point.IsSamePointWithinTolerance(other, orthogonalTolerance));
        }

        public static Point2D RotateAround(this Point2D point, Angle angle, Point2D center)
        {
            // Shift to the origin
            var shiftVector = center.VectorTo(Point2D.Origin);
            var tempPoint = point.TranslateBy(shiftVector);

            // Rotate
            var rotatedPoint = tempPoint.Rotate(angle);

            // Shift back
            return rotatedPoint.TranslateBy(-shiftVector);
            
        }

        public static Point2D TranslateBy(this Point2D point, Vector2D vector)
        {
            return point + vector;
        }

        public static string ToString(this Point2D point, int decimalPrecision)
        {
            return $"({point.X.ToStringWithTrailingZeros(decimalPrecision)}, {point.Y.ToStringWithTrailingZeros(decimalPrecision)})";
        }
    }
}