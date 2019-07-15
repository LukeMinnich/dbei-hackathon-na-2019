using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Kataclysm.Common.Extensions
{
    public static class LineSegment2DExtensions
    {
        private const double COLINEARITYTOLERANCE = 0.0001;
        private const double RANGEOVERLAPTOLERANCE = 0.0001;
        private const double CROSSPRODUCTEQUALITYTOLERANCE = 1e-4;
        private const double DEFAULTBOUNDARYSEGMENTEXTENSION = 1e-8;
        private const double ENDPOINT_COINCIDENCE_TOLERANCE = 1e-6;

        // Per https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
        public static bool CheckForIntersection(this LineSegment2D segment, LineSegment2D other,
            out Point2D? intersection, double thisEpsilon = 1e-10, double otherEpsilon = 1e-10,
            bool extendThisSegment = false, bool extendOtherSegment = false)
        {
            var p = segment.StartPoint;
            var q = other.StartPoint;
            var r = segment.EndPoint - segment.StartPoint;
            var s = other.EndPoint - other.StartPoint;

            var qMinusP = q - p;
            var rCrossS = r.CrossProduct(s);

            intersection = null;

            if (Math.Abs(rCrossS) < CROSSPRODUCTEQUALITYTOLERANCE)
            {
                if (Math.Abs(qMinusP.CrossProduct(r)) < CROSSPRODUCTEQUALITYTOLERANCE)
                {
                    // Lines are collinear
                    var t0 = qMinusP.DotProduct(r) / (r.DotProduct(r));
                    var t1 = t0 + s.DotProduct(r / r.DotProduct(r));

                    var range = s.DotProduct(r) < 0
                        ? new Tuple<double, double>(t1, t0)
                        : new Tuple<double, double>(t0, t1);

                    if (IntervalsOverlap(range, out var overLapAtSinglePoint))
                    {
                        // Lines are collinear and overlapping
                        if (overLapAtSinglePoint)
                        {
                            if (segment.StartPoint.IsSamePointWithinTolerance(other.StartPoint, ENDPOINT_COINCIDENCE_TOLERANCE) ||
                                segment.StartPoint.IsSamePointWithinTolerance(other.EndPoint, ENDPOINT_COINCIDENCE_TOLERANCE))
                                intersection = segment.StartPoint;
                            else if (segment.EndPoint.IsSamePointWithinTolerance(other.StartPoint, ENDPOINT_COINCIDENCE_TOLERANCE) ||
                                     segment.EndPoint.IsSamePointWithinTolerance(other.EndPoint, ENDPOINT_COINCIDENCE_TOLERANCE))
                                intersection = segment.EndPoint;
                        }

                        return true;
                    }
                    else
                    {
                        // Lines are collinear and disjoint
                        return false;
                    }
                }
                else
                {
                    // Lines are parallel
                    return false;
                }
            }

            var t = qMinusP.CrossProduct(s) / rCrossS;
            var u = qMinusP.CrossProduct(r) / rCrossS;

            if ((extendThisSegment || IsBetweenZeroAndOne(t, thisEpsilon)) &&
                (extendOtherSegment || IsBetweenZeroAndOne(u, otherEpsilon)))
            {
                // Lines intersect
                intersection = p + t * r;
                return true;
            }

            // Lines are not parallel but do not intersect
            return false;
        }

        public static bool CheckForIntersectionOfExtension(this LineSegment2D segment, LineSegment2D other,
            out Point2D? intersection)
        {
            return segment.CheckForIntersection(other, out intersection, extendThisSegment: true);
        }

        public static Point2D GetKnownIntersection(this LineSegment2D segment, LineSegment2D other)
        {
            CheckForIntersection(segment, other, out var intersection, extendThisSegment: true,
                extendOtherSegment: true);

            if (intersection == null)
            {
                // Lines are collinear
                Point2D otherMidpoint = other.Midpoint();

                double distanceFromStart = segment.StartPoint.DistanceTo(otherMidpoint);
                double distanceFromEnd = segment.EndPoint.DistanceTo(otherMidpoint);

                return (distanceFromStart < distanceFromEnd)
                    ? segment.StartPoint
                    : segment.EndPoint;
            }

            // Lines are not collinear
            return intersection.Value;
        }

        private static bool IntervalsOverlap(Tuple<double, double> interval, out bool overLapAtSinglePoint)
        {
            overLapAtSinglePoint = (Math.Abs(interval.Item2) < RANGEOVERLAPTOLERANCE ||
                                    Math.Abs(interval.Item1 - 1d) < RANGEOVERLAPTOLERANCE);

            return !(interval.Item2 < 0d) && !(interval.Item1 > 1d);
        }

        private static bool IsBetweenZeroAndOne(double value, double epsilon)
        {
            return 0d - epsilon <= value && value <= 1d + epsilon;
        }

        public static LineSegment2D Offset(this LineSegment2D segment, Offset2DSense sense, double offsetDimension)
        {
            var alignedVector =
                new Vector3D(segment.EndPoint.X - segment.StartPoint.X, segment.EndPoint.Y - segment.StartPoint.Y, 0);

            var unitOffsetVector3D = CalculateUnitOffsetVector(alignedVector, sense);
            var unitOffsetVector2D = new Vector2D(unitOffsetVector3D.X, unitOffsetVector3D.Y);
            var offsetVector = unitOffsetVector2D.ScaleBy(offsetDimension);

            return segment.TranslateBy(offsetVector);
        }
        
        public static LineSegment2D Extend(this LineSegment2D segment, double extensionLength)
        {
            var alignedVector =
                new UnitVector3D(segment.EndPoint.X - segment.StartPoint.X, segment.EndPoint.Y - segment.StartPoint.Y, 0);

            var extendedSegment = new LineSegment2D(segment.StartPoint, new Point2D(segment.EndPoint.X + alignedVector.X * extensionLength, segment.EndPoint.Y + alignedVector.Y * extensionLength));

            return extendedSegment;
        }

        public static PolyLine2D ToPolyLine2D(this LineSegment2D segment)
        {
            return new PolyLine2D(new List<Point2D>() { segment.StartPoint, segment.EndPoint });            
        }

        private static UnitVector3D CalculateUnitOffsetVector(Vector3D alignedVector, Offset2DSense sense)
        {
            Vector3D vectorToCross;

            switch (sense)
            {
                case Offset2DSense.Left:
                    {
                        vectorToCross = new Vector3D(0, 0, -1);
                        break;
                    }
                case Offset2DSense.Right:
                    {
                        vectorToCross = new Vector3D(0, 0, 1);
                        break;
                    }
                default:
                    {
                        throw new InvalidDataException("Offset sense not right nor left");
                    }
            }

            var offsetDirection = alignedVector.CrossProduct(vectorToCross);

            return offsetDirection.Normalize();
        }

        public static Vector2D ToVector2D(this LineSegment2D segment)
        {
            return segment.EndPoint - segment.StartPoint;
        }
        public static Vector2D ToUnitVector2D(this LineSegment2D segment)
        {
            return (segment.EndPoint - segment.StartPoint)/segment.Length;
        }

        public static Side WhichSide(this LineSegment2D segment, Point2D point)
        {
            var v0 = segment.EndPoint - segment.StartPoint;
            var v1 = point - segment.StartPoint;

            var result = v0.CrossProduct(v1);

            if (result < -COLINEARITYTOLERANCE) return Side.Right;
            if (result > COLINEARITYTOLERANCE) return Side.Left;
            return Side.Colinear;
        }

        /// <summary>
        /// Checks for intersections between a line segment and a polygon.  Returns an IEnumerable of the intersection points.
        /// </summary>
        /// <param name="segment">Line segment</param>
        /// <param name="boundary">Polygon boundary</param>
        /// <param name="extend">If set to true, will create an infinite ray along the 'segment' vector and check for intersections between the ray and polygon.  If set to false, will only check the defined line segment.</param>
        /// <returns></returns>
        public static IEnumerable<Point2D> CalculateIntersections(this LineSegment2D segment, Polygon2D boundary,
            bool extend = false)
        {
            var intersections = new List<Point2D>();

            foreach (var edge in boundary.Edges)
            {
                if (segment.CheckForIntersection(edge, out var intersection, thisEpsilon: 0,
                        otherEpsilon: DEFAULTBOUNDARYSEGMENTEXTENSION, extendThisSegment: extend)
                    && intersection != null)
                {
                    intersections.Add(intersection.Value);
                }
            }

            return intersections.Distinct();
        }

        /// <summary>
        /// Returns the intersection points of a line and a convex polygon.  Will also return any line points that are on or within the element (i.e. if the line segement starts within an element, the start point will be returned along with the intersection point)
        /// </summary>
        /// <param name="line">The line segment used to check for intersections</param>
        /// <param name="poly">The convex polygon used to check for intersections</param>
        /// <param name="orthogonalTolerance">Orthogonal distance tolerance</param>
        /// <returns></returns>
        public static List<Point2D> CalculatePointsOnOrInConvexPolygon(this LineSegment2D line, Polygon2D poly, double orthogonalTolerance = COLINEARITYTOLERANCE)
        {
            //First, get all intersections with the polygon boundary
            List<Point2D> lineIntersections = GetUniqueLineIntersections(line.CalculateIntersections(poly, false).ToList(), orthogonalTolerance);

            //If there are less than 2 intersections, check to see if either of the end points is within the polygon
            if (lineIntersections.Count < 2)
            {
                if (WindingNumberInside(line.StartPoint, poly) != 0)
                {
                    //start point of load line is inside element
                    //check to see if the start point is one of the intersections (i.e. start point lies on element boundary)
                    bool included = false;
                    foreach (Point2D p in lineIntersections)
                    {
                        if (p.IsSamePointWithinTolerance(line.StartPoint, orthogonalTolerance))
                        {
                            included = true;
                            break;
                        }
                    }

                    if (included == false)
                    {
                        lineIntersections.Insert(0, line.StartPoint);
                    }
                }

                if (WindingNumberInside(line.EndPoint, poly) != 0)
                {
                    //end point of load line is inside element
                    //check to see if the end point is one of the intersections (i.e. end point lies on element boundary)
                    bool included = false;

                    foreach (Point2D p in lineIntersections)
                    {
                        if (p.IsSamePointWithinTolerance(line.EndPoint, orthogonalTolerance))
                        {
                            included = true;
                            break;
                        }
                    }

                    if (included == false)
                    {
                        lineIntersections.Insert(0, line.EndPoint);
                    }
                }
            }

            //If there are still less than 2 intersections, check if either end point lies on the edge of the polygon
            if (lineIntersections.Count < 2)
            {
                foreach (LineSegment2D edge in poly.Edges)
                {
                    if (line.StartPoint.LiesOnLine(edge))
                    {
                        bool unique = true;

                        foreach (Point2D p in lineIntersections)
                        {
                            if (line.StartPoint.IsSamePointWithinTolerance(p, orthogonalTolerance) == true)
                            {
                                unique = false;
                                break;
                            }
                        }

                        if (unique == true)
                        {
                            lineIntersections.Add(line.StartPoint);
                        }
                    }

                    if (line.EndPoint.LiesOnLine(edge))
                    {
                        bool unique = true;

                        foreach (Point2D p in lineIntersections)
                        {
                            if (line.EndPoint.IsSamePointWithinTolerance(p, orthogonalTolerance) == true)
                            {
                                unique = false;
                                break;
                            }
                        }

                        if (unique == true)
                        {
                            lineIntersections.Add(line.EndPoint);
                        }
                    }
                }
            }

            //If there are still less than 2 intersections, check if either end point is the same point as a polygon vertex
            if (lineIntersections.Count < 2)
            {
                foreach (Point2D v in poly.Vertices)
                {
                    if (line.StartPoint.IsSamePointWithinTolerance(v, orthogonalTolerance) == true)
                    {
                        bool unique = true;

                        foreach (Point2D p in lineIntersections)
                        {
                            if (line.StartPoint.IsSamePointWithinTolerance(p, orthogonalTolerance) == true)
                            {
                                unique = false;
                                break;
                            }
                        }

                        if (unique == true)
                        {
                            lineIntersections.Add(line.StartPoint);
                        }
                    }

                    if (line.EndPoint.IsSamePointWithinTolerance(v, orthogonalTolerance) == true)
                    {
                        bool unique = true;

                        foreach (Point2D p in lineIntersections)
                        {
                            if (line.EndPoint.IsSamePointWithinTolerance(p, orthogonalTolerance) == true)
                            {
                                unique = false;
                                break;
                            }
                        }

                        if (unique == true)
                        {
                            lineIntersections.Add(line.EndPoint);
                        }
                    }
                }
            }

            return lineIntersections;
        }

        private static List<Point2D> GetUniqueLineIntersections(List<Point2D> intersections, double orthogonalTolerance)
        {
            List<Point2D> uniqueLineLoadIntersections = new List<Point2D>(intersections.Count);

            foreach (Point2D p in intersections)
            {
                bool unique = true;

                foreach (Point2D up in uniqueLineLoadIntersections)
                {
                    if (p.IsSamePointWithinTolerance(up, orthogonalTolerance))
                    {
                        unique = false;
                        break;
                    }
                }

                if (unique == true)
                {
                    uniqueLineLoadIntersections.Add(p);
                }
            }

            return uniqueLineLoadIntersections;
        }

        public static bool OverlapsBoundary(this LineSegment2D segment, Polygon2D boundary)
        {
            foreach (var edge in boundary.Edges)
            {
                var intersects = segment.CheckForIntersection(edge, out var intersection);

                if (intersects && intersection == null) return true;
            }

            return false;
        }

        public static Point2D Midpoint(this LineSegment2D segment)
        {
            return new Point2D((segment.StartPoint.X + segment.EndPoint.X) / 2,
                (segment.StartPoint.Y + segment.EndPoint.Y) / 2);
        }

        public static Vector2D CalculateUnitVector(this LineSegment2D segment)
        {
            return segment.ToVector2D().Normalize();
        }

        /// <summary>
        /// Calculates the angle of the line segment in degrees
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static double CalculateAngle(this LineSegment2D segment)
        {
            double angle;
            Vector2D wallUnitVector = segment.CalculateUnitVector();

            if (wallUnitVector.X == 0)
            {
                if (wallUnitVector.Y > 0)
                {
                    angle = 90;
                }
                else
                {
                    angle = -90;
                }
            }
            else
            {
                angle = Math.Atan(wallUnitVector.Y / wallUnitVector.X) * 180 / Math.PI;
            }

            return angle;
        }

        public static bool IsColinear(this LineSegment2D seg1, LineSegment2D seg2, double tolerance, out Point2D? int1,
            out Point2D? int2)
        {
            double edge1_xmin = Math.Min(seg1.StartPoint.X, seg1.EndPoint.X);
            double edge1_ymin = Math.Min(seg1.StartPoint.Y, seg1.EndPoint.Y);
            double edge1_xmax = Math.Max(seg1.StartPoint.X, seg1.EndPoint.X);
            double edge1_ymax = Math.Max(seg1.StartPoint.Y, seg1.EndPoint.Y);

            double edge2_xmin = Math.Min(seg2.StartPoint.X, seg2.EndPoint.X);
            double edge2_ymin = Math.Min(seg2.StartPoint.Y, seg2.EndPoint.Y);
            double edge2_xmax = Math.Max(seg2.StartPoint.X, seg2.EndPoint.X);
            double edge2_ymax = Math.Max(seg2.StartPoint.Y, seg2.EndPoint.Y);


            if (Math.Abs(seg1.StartPoint.X - seg1.EndPoint.X) <= tolerance)
            {
                if (Math.Abs(seg2.StartPoint.X - seg2.EndPoint.X) <= tolerance)
                {
                    if (Math.Abs(seg1.StartPoint.X - seg2.StartPoint.X) <= tolerance)
                    {
                        if ((edge1_ymin >= edge2_ymin || Math.Abs(edge1_ymin - edge2_ymin) <= tolerance) && edge1_ymin < edge2_ymax)
                        {
                            int1 = new Point2D(edge1_xmin, edge1_ymin);

                            if (edge1_ymax <= edge2_ymax || Math.Abs(edge1_ymax - edge2_ymax) <= tolerance)
                            {
                                int2 = new Point2D(edge1_xmax, edge1_ymax);
                            }
                            else
                            {
                                int2 = new Point2D(edge2_xmax, edge2_ymax);
                            }

                            return true;
                        }
                        else if ((edge2_ymin >= edge1_ymin || Math.Abs(edge2_ymin - edge1_ymin) <= tolerance) && edge2_ymin < edge1_ymax)
                        {
                            int1 = new Point2D(edge2_xmin, edge2_ymin);

                            if (edge2_ymax <= edge1_ymax || Math.Abs(edge2_ymax - edge1_ymax) <= tolerance)
                            {
                                int2 = new Point2D(edge2_xmax, edge2_ymax);
                            }
                            else
                            {
                                int2 = new Point2D(edge1_xmax, edge1_ymax);
                            }

                            return true;
                        }
                    }
                }
            }
            else
            {
                double m1 = (seg1.EndPoint.Y - seg1.StartPoint.Y) / (seg1.EndPoint.X - seg1.StartPoint.X);
                double b1 = seg1.StartPoint.Y - m1 * seg1.StartPoint.X;

                double maxSlopeDiff = (2.0 * tolerance) / (Math.Abs(seg1.EndPoint.X - seg1.StartPoint.X) - 2.0 * tolerance);

                if (Math.Abs(seg2.StartPoint.X - seg2.EndPoint.X) > tolerance)
                {
                    double m2 = (seg2.EndPoint.Y - seg2.StartPoint.Y) / (seg2.EndPoint.X - seg2.StartPoint.X);
                    double b2 = seg2.StartPoint.Y - m2 * seg2.StartPoint.X;

                    if (Math.Abs(b2 - b1) <= tolerance && Math.Abs(m2 - m1) <= maxSlopeDiff)
                    {
                        if ((edge2_xmin >= edge1_xmin || Math.Abs(edge2_xmin - edge1_xmin) <= tolerance) && edge2_xmin < edge1_xmax)
                        {
                            int1 = new Point2D(edge2_xmin, m2 * edge2_xmin + b2);

                            if (edge2_xmax <= edge1_xmax || Math.Abs(edge2_xmax - edge1_xmax) <= tolerance)
                            {
                                int2 = new Point2D(edge2_xmax, m2 * edge2_xmax + b2);
                            }
                            else
                            {
                                int2 = new Point2D(edge1_xmax, m1 * edge1_xmax + b1);
                            }

                            return true;
                        }
                        else if ((edge1_xmin >= edge2_xmin || Math.Abs(edge1_xmin - edge2_xmin) <= tolerance) && edge1_xmin < edge2_xmax)
                        {
                            int1 = new Point2D(edge1_xmin, m1 * edge1_xmin + b1);

                            if (edge1_xmax <= edge2_xmax || Math.Abs(edge1_xmax - edge2_xmax) <= tolerance)
                            {
                                int2 = new Point2D(edge1_xmax, m1 * edge1_xmax + b1);
                            }
                            else
                            {
                                int2 = new Point2D(edge2_xmax, m2 * edge2_xmax + b2);
                            }

                            return true;
                        }
                    }

                }
            }

            int1 = null;
            int2 = null;
            return false;
        }

        public static LineSegment2D Rotate(this LineSegment2D segment, Angle angle)
        {
            var vectorA = segment.StartPoint.ToVector2D();
            var vectorB = segment.EndPoint.ToVector2D();

            return new LineSegment2D(vectorA.Rotate(angle).ToPoint2D(), vectorB.Rotate(angle).ToPoint2D());
        }

        public static Point2D ClosestPointToExtension(this LineSegment2D segment, Point2D point)
        {
            var start = segment.StartPoint;
            var end = segment.EndPoint;

            // Vertical line segment
            if (Math.Abs(start.X - end.X) < 1e-10)
            {
                return new Point2D(start.X, point.Y);
            }

            // Horizontal line segment
            if (Math.Abs(start.Y - end.Y) < 1e-10)
            {
                return new Point2D(point.X, start.Y);
            }

            // Line intersection based on point-slope equation of a line
            var m1 = (end.Y - start.Y) / (end.X - start.X);
            var m2 = -1 / m1;

            var x1 = start.X;
            var y1 = start.Y;

            var x2 = point.X;
            var y2 = point.Y;

            var x = ((m1 * x1 - m2 * x2) - (y1 - y2)) / (m1 - m2);
            var y = m1 * x + y1 - m1 * x1;

            return new Point2D(x, y);
        }

        public static List<LineSegment2D> SubdivideAlongXAxisAtVertices(this LineSegment2D segment,
            IEnumerable<Point2D> vertices)
        {
            var verticesWithUniqueAbscissas = new Dictionary<string, Point2D>();

            foreach (var vertex in vertices)
            {
                var abscissa = $"{vertex.Y:0.0000000000}";

                if (!verticesWithUniqueAbscissas.ContainsKey(abscissa))
                {
                    verticesWithUniqueAbscissas.Add(abscissa, vertex);
                }
            }

            var subSegments = new List<LineSegment2D>() { new LineSegment2D(segment.StartPoint, segment.EndPoint) };

            foreach (var vertex in verticesWithUniqueAbscissas.Values)
            {
                var splitSegment = new LineSegment2D(vertex, vertex + Vector2D.XAxis);

                subSegments = SubdivideAll(subSegments, splitSegment);
            }

            return subSegments;
        }

        private static List<LineSegment2D> SubdivideAll(IEnumerable<LineSegment2D> segments, LineSegment2D divider)
        {
            var subdivisions = new List<LineSegment2D>();

            foreach (var segment in segments)
            {
                if (divider.CheckForIntersectionOfExtension(segment, out var intersection) && intersection != null &&
                    intersection != segment.StartPoint && intersection != segment.EndPoint)
                {
                    subdivisions.Add(new LineSegment2D(segment.StartPoint, intersection.Value));
                    subdivisions.Add(new LineSegment2D(intersection.Value, segment.EndPoint));
                }
                else
                {
                    subdivisions.Add(segment);
                }
            }

            return subdivisions;
        }

        public static Point2D MinOrMaxAbscissaPoint(this LineSegment2D segment, bool minimum)
        {
            var start = segment.StartPoint;
            var end = segment.EndPoint;

            return ((minimum && start.Y < end.Y) || (!minimum && start.Y > end.Y))
                ? start
                : end;
        }

        public static double MinimumX(this LineSegment2D segment)
        {
            return Math.Min(segment.StartPoint.X, segment.EndPoint.X);
        }
        public static double MaximumX(this LineSegment2D segment)
        {
            return Math.Max(segment.StartPoint.X, segment.EndPoint.X);
        }
        public static double MinimumY(this LineSegment2D segment)
        {
            return Math.Min(segment.StartPoint.Y, segment.EndPoint.Y);
        }
        public static double MaximumY(this LineSegment2D segment)
        {
            return Math.Max(segment.StartPoint.Y, segment.EndPoint.Y);
        }


        public static List<LineSegment2D> SubdivideAtCollinearPoints(this LineSegment2D line,
            IEnumerable<Point2D> unorderedPoints)
        {
            var allUnorderedVertices = unorderedPoints.Where(p => p.IsOn(line)).ToList();

            allUnorderedVertices.Add(line.StartPoint);
            allUnorderedVertices.Add(line.EndPoint);

            var culledUnorderedPoints = allUnorderedVertices.Distinct().ToList();

            var orderedVertices = culledUnorderedPoints.OrderCollinearPoints();

            var subdivisions = new List<LineSegment2D>();

            for (int i = 0; i < orderedVertices.Count - 1; i++)
            {
                subdivisions.Add(new LineSegment2D(orderedVertices[i], orderedVertices[i + 1]));
            }

            return subdivisions;
        }

        public static bool IsOn(this LineSegment2D line, Polygon2D boundary)
        {
            foreach (var edge in boundary.Edges)
            {
                if (line.StartPoint.IsOn(edge) && line.EndPoint.IsOn(edge)) return true;
            }

            return false;
        }

        public static LineSegment2D? OverlapWith(this LineSegment2D line, LineSegment2D other)
        {
            List<LineSegment2D> subSegments = line.SubdivideAtCollinearPoints(new List<Point2D>()
            {
                other.StartPoint,
                other.EndPoint
            });

            foreach (var subSegment in subSegments)
            {
                var midpoint = subSegment.Midpoint();

                if (midpoint.IsOn(line) && midpoint.IsOn(other)) return subSegment;
            }

            return null;
        }

        public static bool StartsOrEndsAt(this LineSegment2D line, Point2D point)
        {
            return (line.StartPoint == point || line.EndPoint == point);
        }

        public static double GetDistanceFromPointToLine(this LineSegment2D line, Point2D p0)
        {
            double x1 = line.StartPoint.X;
            double y1 = line.StartPoint.Y;
            double x2 = line.EndPoint.X;
            double y2 = line.EndPoint.Y;
            double x0 = p0.X;
            double y0 = p0.Y;

            if (Math.Abs(x2 - x1) < 1e-10 && Math.Abs(y2 - y1) < 1e-10)
            {
                //This is a vertical element that would create simply a point (not a line) in the X-Y plane
                return Math.Sqrt(Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2));
            }
            else
            {
                return Math.Abs((x2 - x1) * (y1 - y0) - (x1 - x0) * (y2 - y1)) / Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            }
        }

        public static Point2D ClosestEnd(this LineSegment2D segment, Point2D point)
        {
            double distanceFromStart = segment.StartPoint.DistanceTo(point);
            double distanceFromEnd = segment.EndPoint.DistanceTo(point);

            return (distanceFromStart < distanceFromEnd)
                ? segment.StartPoint
                : segment.EndPoint;
        }

        public static LineSegment2D RotateAround(this LineSegment2D segment, Angle angle, Point2D center)
        {
            // Shift to the origin
            var shiftVector = center.VectorTo(Point2D.Origin);
            var tempSegment = segment.TranslateBy(shiftVector);

            // Rotate
            var rotatedSegment = tempSegment.Rotate(angle);

            // Shift back
            return rotatedSegment.TranslateBy(-shiftVector);

        }
    }
}