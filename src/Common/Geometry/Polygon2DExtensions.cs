using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Kataclysm.Common.Extensions;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Kataclysm.Common.Geometry
{
    public static class Polygon2DExtensions
    {
        private static double _epsilon = 1.0E-7;

        public static bool IsConvex(this Polygon2D polygon)
        {
            var convexHull = Polygon2D.GetConvexHullFromPoints(polygon.Vertices);

            return polygon.VertexCount == convexHull.VertexCount;
        }

        public static Polygon2D Reverse(this Polygon2D polygon)
        {
            return new Polygon2D(polygon.Vertices.Reverse());
        }

        // Per https://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
        public static bool IsClockwise(this Polygon2D polygon)
        {
            return GetSignedArea(polygon) > 0d;
        }

        // Per https://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
        public static double GetArea(this Polygon2D polygon)
        {
            return Math.Abs(GetSignedArea(polygon));
        }

        // Per https://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
        private static double GetSignedArea(Polygon2D polygon)
        {
            double sum = 0;

            foreach (var edge in polygon.Edges)
            {
                var pointA = edge.StartPoint;
                var pointB = edge.EndPoint;

                sum += (pointB.X - pointA.X) * (pointB.Y + pointA.Y);
            }

            return sum / 2;
        }

        public static double SignedArea(this Polygon2D polygon)
        {
            double sum = 0;

            List<Point2D> vertices = polygon.Vertices.ToList();

            for (int i = 0; i < vertices.Count; i++)
            {
                Point2D p0 = vertices[i];

                Point2D p1;

                if (i == vertices.Count - 1)
                {
                    p1 = vertices[0];
                }
                else
                {
                    p1 = vertices[i + 1];
                }

                sum += (p0.X * p1.Y - p1.X * p0.Y);
            }

            return sum / 2.0;
        }

        public static Point2D GetCentroid(this Polygon2D polygon)
        // use Archimedes.geometry formulation
        {
            double sumX = 0;
            double sumY = 0;

            var vertices = polygon.Vertices.ToList();

            for (int i = 0; i < vertices.Count; i++)
            {
                Point2D p0 = vertices[i];
                Point2D p1;

                if (i < vertices.Count - 1)
                {
                    p1 = vertices[i + 1];
                }
                else
                {
                    p1 = vertices[0];
                }

                sumX += (p0.X + p1.X) * (p0.X * p1.Y - p1.X * p0.Y);

                sumY += (p0.Y + p1.Y) * (p0.X * p1.Y - p1.X * p0.Y);
            }

            double area = polygon.SignedArea();

            double cx = 1.0 / (6.0 * area) * sumX;

            double cy = 1.0 / (6.0 * area) * sumY;

            return new Point2D(cx, cy);
        }

        public static bool EnclosesPointIncludingEdges(this Polygon2D polygon, Point2D point)
        {
            if (polygon.EnclosesPoint(point)) return true;

            foreach (var edge in polygon.Edges)
            {
                if (point.IsOn(edge)) return true;
            }

            return false;
        }

        public static List<Polygon2D> BooleanIntersection(this Polygon2D boundary, Polygon2D other)
        {
            var convertedBoundary = ConvertToPolyBoolPolygon(boundary);
            var convertedOther = ConvertToPolyBoolPolygon(other);

            var polybool = new PolyBool();

            Polygon intersection = polybool.intersect(convertedBoundary, convertedOther);

            if (intersection.regions.Any())
            {
                var overlappingPolygons = new List<Polygon2D>();

                foreach (var region in intersection.regions)
                {
                    if (region.Count != 0)
                    {
                        overlappingPolygons.AddRange(ConvertFromPolyBoolPolygon(region));
                    }
                }

                return overlappingPolygons;
            }
            else
            {
                if (convertedBoundary.regions.Count == 1)
                {
                    if (convertedOther.regions.Count == 1)
                    {
                        if (AreRegionsExactlyTheSame(convertedBoundary.regions[0], convertedOther.regions[0]) == true)
                        {
                            var overlappingPolygons = new List<Polygon2D>();

                            overlappingPolygons.AddRange(ConvertFromPolyBoolPolygon(convertedBoundary.regions[0]));

                            return overlappingPolygons;
                        }
                    }
                }
            }

            return new List<Polygon2D>();
        }

        private static bool AreRegionsExactlyTheSame(PointList region1, PointList region2)
        {
            bool same = true;

            if (region1.Count == region2.Count)
            {
                for (int i = 0; i< region1.Count; i++)
                {
                    if (region1[i] != region2[i])
                    {
                        same = false;
                        break;
                    }
                }
            }
            else
            {
                same = false;
            }

            if (same == false)
            {
                PointList region1_R = new PointList(); //resampled
                PointList region2_R = new PointList(); //resampled

                for (int i = 0; i< region1.Count; i++)
                {
                    region1_R.Add(region1[i]);
                }

                for (int i = 0; i < region2.Count; i++)
                {
                    region2_R.Add(region2[i]);
                }

                int count = 0;

                while (count <= region1_R.Count - 3)
                {
                    Point p1 = region1_R[count];
                    Point p2 = region1_R[count + 1];
                    Point p3 = region1_R[count + 2];

                    double m12, m23;

                    if (p1.x == p2.x)
                    {
                        if (p2.x == p3.x)
                        {
                            //slopes are the same, remove point 2
                            region1_R.RemoveAt(count + 1);
                            count--;
                        }
                    }
                    else
                    {
                        m12 = (p2.y - p1.y) / (p2.x - p1.x);
                        if (p2.x != p3.x)
                        {
                            m23 = (p3.y - p2.y) / (p3.x - p2.x);
                            if (m12 == m23)
                            {
                                region1_R.RemoveAt(count + 1);
                                count--;
                            }
                        }
                    }

                    count++;
                }

                count = 0;

                while (count <= region2_R.Count - 3)
                {
                    Point p1 = region2_R[count];
                    Point p2 = region2_R[count + 1];
                    Point p3 = region2_R[count + 2];

                    double m12, m23;

                    if (p1.x == p2.x)
                    {
                        if (p2.x == p3.x)
                        {
                            //slopes are the same, remove point 2
                            region2_R.RemoveAt(count + 1);
                            count--;
                        }
                    }
                    else
                    {
                        m12 = (p2.y - p1.y) / (p2.x - p1.x);
                        if (p2.x != p3.x)
                        {
                            m23 = (p3.y - p2.y) / (p3.x - p2.x);
                            if (m12 == m23)
                            {
                                region2_R.RemoveAt(count + 1);
                                count--;
                            }
                        }
                    }

                    count++;
                }

                //Now both polygons are resampled

                if (region1_R.Count != region2_R.Count)
                {
                    return false;
                }
                else
                {
                    int numSame = 0;

                    foreach (Point p1 in region1_R)
                    {
                        foreach (Point p2 in region2_R)
                        {
                            if (p1 == p2)
                            {
                                numSame++;
                                break;
                            }
                        }
                    }

                    if (numSame == region1_R.Count)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            else
            {
                return true;
            }
        }

        public static Polygon2D BooleanUnion(this IEnumerable<Polygon2D> polygons, double orthogonalTolerance)
        {
            List<PolyBoolCS.Polygon> polyBoolPolygons = new List<PolyBoolCS.Polygon>();

            foreach (Polygon2D polygon in polygons)
            {
                List<PolyBoolCS.Point> pointList = new List<PolyBoolCS.Point>();

                List<Point2D> polygonPoints = polygon.Vertices.ToList();

                foreach (Point2D point in polygonPoints)
                {
                    pointList.Add(new PolyBoolCS.Point(point.X, point.Y));
                }

                polyBoolPolygons.Add(PolyBoolHelper.ConvertToPolygon(pointList));
            }

            PreProcessor helper = new PreProcessor(orthogonalTolerance);
            PolyBoolCS.Polygon unionedPolygon = helper.GetUnionOfPolygons(polyBoolPolygons);

            List<Polygon2D> unionedPolygon2D = ConvertPolyBoolRegionsToPolygon2D(unionedPolygon);

            if (unionedPolygon2D.Count > 1)
            {
                Polygon2D largest = unionedPolygon2D.OrderByDescending(p => p.GetArea()).First();

                foreach (Polygon2D boundary in unionedPolygon2D)
                {
                    if (boundary == largest) continue;

                    List<Polygon2D> diff = boundary.BooleanDifference(largest);
                    
                    if (diff.Count > 0) 
                        throw new Exception(
                            "Level diaphragm boundary contains more than one unioned polygon.  To use rigid daphragm analysis, there must be one continuous diaphragm.");
                }

                return largest;
            }
            else
            {
                return unionedPolygon2D[0];
            }
        }

        private static List<Polygon2D> ConvertPolyBoolRegionsToPolygon2D(PolyBoolCS.Polygon poly)
        {
            List<Polygon2D> shapeList = new List<Polygon2D>(poly.regions.Count);

            foreach (PolyBoolCS.PointList region in poly.regions)
            {
                List<PolyBoolCS.Point> pointList = PolyBoolHelper.ConvertRegionToPointList(region);

                List<Point2D> point2Dlist = new List<Point2D>(pointList.Count);

                foreach (PolyBoolCS.Point p in pointList)
                {
                    point2Dlist.Add(new Point2D(p.x, p.y));
                }

                shapeList.Add(new Polygon2D(point2Dlist));
            }

            return shapeList;
        }

        public static List<Polygon2D> BooleanDifference(this Polygon2D boundary, Polygon2D other)
        {
            var convertedBoundary = ConvertToPolyBoolPolygon(boundary);
            var convertedOther = ConvertToPolyBoolPolygon(other);

            var polybool = new PolyBool();

            Polygon difference = polybool.difference(convertedBoundary, convertedOther);

            if (difference.regions.Any())
            {
                var differencePolygons = new List<Polygon2D>();

                foreach (var region in difference.regions)
                {
                    differencePolygons.AddRange(ConvertFromPolyBoolPolygon(region));
                }

                return differencePolygons;
            }

            return new List<Polygon2D>();
        }

        public static Polygon ConvertToPolyBoolPolygon(this Polygon2D boundary)
        {
            var convertedVertices = ConvertToPolyBoolPoints(boundary.Vertices);

            return PolyBoolHelper.ConvertToPolygon(convertedVertices.ToList());
        }

        public static List<Polygon2D> ConvertFromPolyBoolPolygon(PointList region)
        {
            List<Point> vertices = PolyBoolHelper.ConvertRegionToPointList(region);

            var convertedVertices = ConvertFromPolyBoolPoints(vertices).ToList();

            var convertedBoundaries = new List<Polygon2D>();

            try
            {
                SimplePolygon2D convertedPolygon = new SimplePolygon2D(convertedVertices);

                convertedBoundaries.Add(convertedPolygon);
            }
            catch (SimplePolygonException)
            {
                var complexPolygon = new Polygon2D(convertedVertices);

                convertedBoundaries.AddRange(complexPolygon.SubdivideComplexPolygon());
            }

            return convertedBoundaries;
        }

        public static List<Polygon2D> CookieCutFromLineExtension(this Polygon2D boundary,
            LineSegment2D line)
        {
            var rotation = line.ToVector2D().CounterClockwiseRotationToXAxis();

            var rotatedBoundary = boundary.Rotate(rotation);
            var rotatedLine = line.Rotate(rotation);

            var leftOffset = 0d;
            var rightOffset = 0d;

            Point2D minOrdinateVertex = rotatedLine.StartPoint;
            Point2D maxOrdinateVertex = rotatedLine.EndPoint;

            foreach (var vertex in rotatedBoundary.Vertices)
            {
                var distanceFromLineExtension = vertex.DistanceToLineExtension(rotatedLine);

                switch (rotatedLine.WhichSide(vertex))
                {
                    case Side.Right:
                        if (distanceFromLineExtension > rightOffset) rightOffset = distanceFromLineExtension;
                        break;
                    case Side.Left:
                        if (distanceFromLineExtension > leftOffset) leftOffset = distanceFromLineExtension;
                        break;
                    case Side.Colinear:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (vertex.X < minOrdinateVertex.X) minOrdinateVertex = vertex;
                if (vertex.X > maxOrdinateVertex.X) maxOrdinateVertex = vertex;
            }

            var extendedLine = new LineSegment2D(rotatedLine.ClosestPointToExtension(minOrdinateVertex) - Vector2D.XAxis,
                rotatedLine.ClosestPointToExtension(maxOrdinateVertex) + Vector2D.XAxis);

            var rotatedRegions = new List<Polygon2D>();

            if (leftOffset > 0d)
            {
                var leftOffsetLine = extendedLine.Offset(Offset2DSense.Left, leftOffset);

                var leftBoundingRegion = new Polygon2D(new List<Point2D>()
                {
                    extendedLine.EndPoint,
                    extendedLine.StartPoint,
                    leftOffsetLine.StartPoint,
                    leftOffsetLine.EndPoint
                });

                var overlappingRegions = rotatedBoundary.BooleanIntersection(leftBoundingRegion);

                if (overlappingRegions.Any()) rotatedRegions.AddRange(overlappingRegions);
            }

            if (rightOffset > 0d)
            {
                var rightOffsetLine = extendedLine.Offset(Offset2DSense.Right, rightOffset);

                var rightBoundingRegion = new Polygon2D(new List<Point2D>()
                {
                    extendedLine.StartPoint,
                    extendedLine.EndPoint,
                    rightOffsetLine.EndPoint,
                    rightOffsetLine.StartPoint
                });

                var overlappingRegions = rotatedBoundary.BooleanIntersection(rightBoundingRegion);

                if (overlappingRegions.Any()) rotatedRegions.AddRange(overlappingRegions);
            }

            return rotatedRegions.Select(r => r.Rotate(-1 * rotation)).ToList();
        }

        public static List<Polygon2D> SubdivideAlongXAxisAtVertices(this Polygon2D boundary,
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

            var subBoundaries = new List<Polygon2D>() { new Polygon2D(boundary.Vertices) };

            foreach (var vertex in verticesWithUniqueAbscissas.Values)
            {
                var splitSegment = new LineSegment2D(vertex, vertex + Vector2D.XAxis);

                subBoundaries = SubdivideAll(subBoundaries, splitSegment);
            }

            return subBoundaries;
        }

        private static List<Polygon2D> SubdivideAll(IEnumerable<Polygon2D> boundaries, LineSegment2D divider)
        {
            var subdivisions = new List<Polygon2D>();

            foreach (var boundary in boundaries)
            {
                subdivisions.AddRange(boundary.CookieCutFromLineExtension(divider));
            }

            return subdivisions;
        }

        public static Point2D MinOrMaxAbscissaPoint(this Polygon2D boundary, bool minimum)
        {
            var vertices = boundary.Vertices.ToList();

            var result = new Tuple<double, Point2D>(vertices[0].Y, vertices[0]);

            foreach (var vertex in vertices)
            {
                if ((minimum && vertex.Y < result.Item1) || (!minimum && vertex.Y > result.Item1))
                    result = new Tuple<double, Point2D>(vertex.Y, vertex);
            }

            return result.Item2;
        }

        public static Point2D BisectionPoint(this Polygon2D boundary, Point2D fromVertex)
        {
            List<Point2D> vertices = boundary.Vertices.ToList();
            var vertexCount = vertices.Count;

            var vertexIndex = vertices.IndexOf(fromVertex);

            Point2D bisectionPoint;

            if (vertexCount % 2 == 0)
            {
                var bisectionPointIndex = vertexIndex + vertexCount / 2;

                if (bisectionPointIndex > vertexCount - 1) bisectionPointIndex = bisectionPointIndex - vertexCount;

                bisectionPoint = vertices[bisectionPointIndex];
            }
            else
            {
                var indexBeforeBisectionPoint = vertexIndex + vertexCount / 2;
                var indexAfterBisectionPoint = indexBeforeBisectionPoint + 1;

                if (indexBeforeBisectionPoint > vertexCount - 1)
                    indexBeforeBisectionPoint = indexBeforeBisectionPoint - vertexCount;

                if (indexAfterBisectionPoint > vertexCount - 1)
                    indexAfterBisectionPoint = indexAfterBisectionPoint - vertexCount;

                var vertexBefore = vertices[indexBeforeBisectionPoint];
                var vertexAfter = vertices[indexAfterBisectionPoint];

                bisectionPoint = vertexBefore + 0.5 * vertexBefore.VectorTo(vertexAfter);
            }

            return bisectionPoint;
        }

        public static Vector2D Bisect(this Polygon2D boundary, Point2D fromVertex)
        {
            var point = boundary.BisectionPoint(fromVertex);

            return fromVertex.VectorTo(point);
        }

        public static PrePolygon ToPrePolygon(this Polygon2D boundary)
        {
            IEnumerable<PreVertex> vertices = boundary.Vertices.ConvertToPreVertices();

            return new PrePolygon(vertices);
        }

        public static List<Polygon2D> ConvertFromPrePolygons(IEnumerable<PrePolygon> meshBoundaries)
        {
            var convertedMeshes = new List<Polygon2D>();

            var boundaries = meshBoundaries.ToList();

            foreach (var boundary in boundaries)
            {
                convertedMeshes.Add(ConvertFromPrePolygon(boundary));
            }

            return convertedMeshes;
        }

        public static Polygon2D ConvertFromPrePolygon(this PrePolygon meshBoundary)
        {
            IEnumerable<Point2D> vertices = Point2DExtensions.ConvertFromPreVertices(meshBoundary.Vertices);

            return new Polygon2D(vertices);
        }

        public static List<SimplePolygon2D> SubdivideComplexPolygon(this Polygon2D boundary)
        {
            List<Point2D> intersections = boundary.SelfIntersections(epsilon: 1e-7);

            List<Point2D> nudgedVertices = boundary.Vertices.ReplaceBasedOnDistanceApart(intersections, epsilon: 1e-7);

            List<Point2D> allVertices = InsertVertices(nudgedVertices, intersections);

            List<Polygon2D> buddedPolygons = BudPolygonsFromSourceVertices(allVertices, intersections);

            return ReduceComplexity(buddedPolygons);
        }

        public static List<Point2D> SelfIntersections(this Polygon2D boundary, double epsilon = 1e-7)
        {
            List<Point2D> intersections = AllSelfIntersections(boundary);

            return intersections.DistinctBasedOnDistanceApart(epsilon);
        }

        private static List<Point2D> AllSelfIntersections(Polygon2D boundary)
        {
            var intersections = new List<Point2D>();

            var edges = boundary.Edges.ToList();

            for (var i = 0; i < edges.Count; i++)
            {
                var thisEdge = edges[i];

                // Avoid comparing next and previous edges
                for (var j = 2; j < edges.Count - 1; j++)
                {
                    var otherEdgeIndex = i + j;

                    if (otherEdgeIndex > edges.Count - 1) otherEdgeIndex = otherEdgeIndex - edges.Count;

                    var otherEdge = edges[otherEdgeIndex];

                    if (thisEdge.CheckForIntersection(otherEdge, out var intersection) && intersection != null)
                    {
                        intersections.Add(intersection.Value);
                    }
                }
            }

            return intersections;
        }

        private static List<Point2D> InsertVertices(IEnumerable<Point2D> vertices, IEnumerable<Point2D> intersections)
        {
            var boundary = new Polygon2D(vertices);

            var allIntersections = intersections.ToList();

            var verticesWithInsertions = new LinkedList<Point2D>(boundary.Vertices);

            foreach (var edge in boundary.Edges)
            {
                foreach (var intersection in allIntersections)
                {
                    if (!edge.StartsOrEndsAt(intersection) && intersection.IsOn(edge))
                    {
                        LinkedListNode<Point2D> startNode = verticesWithInsertions.Find(edge.StartPoint);

                        Debug.Assert(startNode != null, nameof(startNode) + " != null");

                        verticesWithInsertions.AddAfter(startNode, intersection);
                    }
                }
            }

            return verticesWithInsertions.ToList();
        }

        private static List<Polygon2D> BudPolygonsFromSourceVertices(IEnumerable<Point2D> vertices,
            IEnumerable<Point2D> intersections)
        {
            var culledVertices = new LinkedList<Point2D>(vertices);
            var allIntersections = intersections.ToList();

            var item = culledVertices.First;

            var polygons = new List<Polygon2D>();

            LinkedListNode<Point2D> budStart = null;

            while (item != null)
            {
                var nextItem = item.Next;

                if (allIntersections.Contains(item.Value))
                {
                    if (budStart == null)
                    {
                        budStart = item;
                    }
                    else if (item.Value == budStart.Value)
                    {
                        List<Point2D> budVertices = culledVertices.GetBetween(budStart, item.Previous, false).ToList();

                        var culledIntersections = allIntersections.Where(i => i != budStart.Value).ToList();

                        // Given a small epsilon in Polybool, there is a chance that a bud might occur
                        //   that is just a line segment out and back to the intersection point.
                        if (budVertices.Count < 3)
                        {
                            culledVertices.RemoveFirstInstanceOfDistinctIn(budVertices, culledVertices.Find(budVertices.First()));
                        }
                        else
                        {
                            List<Polygon2D> buds = BudPolygonsFromSourceVertices(budVertices, culledIntersections);

                            foreach (var bud in buds)
                            {
                                // remove bud nodes only when the bud is not a hole
                                if (bud.IsClockwise())
                                {
                                    if (bud.Equals(buds.Last())) nextItem = item;

                                    polygons.Add(bud);

                                    culledVertices.RemoveFirstInstanceOfDistinctIn(bud.Vertices,
                                        culledVertices.Find(bud.Vertices.First()));
                                }
                                else
                                {
                                    if (bud.Equals(buds.Last())) nextItem = budStart.Next;
                                }
                            }

                            budStart = null;
                        } 
                    }
                }

                item = nextItem;
            }

            polygons.Add(new SimplePolygon2D(culledVertices));

            return polygons;
        }

        private static List<SimplePolygon2D> ReduceComplexity(IEnumerable<Polygon2D> boundaries)
        {
            var simplified = new List<SimplePolygon2D>();

            foreach (var boundary in boundaries)
            {
                simplified.Add(new SimplePolygon2D(boundary.ReduceComplexity(1e-6).Vertices));
            }

            return simplified;
        }

        /// <summary>
        /// Offsets a polygon by a given distance
        /// </summary>
        /// <param name="polygon">The polygon to be offset</param>
        /// <param name="offsetDistance">The offset distance (enter a positive number)</param>
        /// <param name="direction">For interior offset, specify 1, for exterior offset, specify -1</param>
        /// <returns></returns>
        public static Polygon2D Offset(this Polygon2D polygon, double offsetDistance, int direction)
        {

            List<Point2D> pointList = polygon.Vertices.ToList();

            List<Point2D> offsetPointList = new List<Point2D>(pointList.Count);

            for (int i = 0; i < pointList.Count; i++)
            {
                Point2D prevPoint;
                Point2D point;
                Point2D nextPoint;

                int prevIndex = 0;
                int index = 0;
                int nextIndex = 0;

                if (i == 0)
                {
                    prevIndex = pointList.Count - 1;
                    index = i;
                    nextIndex = i + 1;
                }
                else if (i > 0 && i < pointList.Count - 1)
                {
                    prevIndex = i - 1;
                    index = i;
                    nextIndex = i + 1;
                }
                else
                {
                    prevIndex = i - 1;
                    index = i;
                    nextIndex = 0;
                }

                prevPoint = pointList[prevIndex];
                point = pointList[index];
                nextPoint = pointList[nextIndex];

                double angle1 = point.GetAngleTo(nextPoint);
                double angle2 = point.GetAngleTo(prevPoint);

                double avgAngle = (angle1 + angle2) / 2d;

                double angleDiff = angle2 - angle1;

                double halfTheta = angleDiff / 2d;

                double totalDist = Math.Abs(offsetDistance / Math.Sin(halfTheta));

                Point2D newPoint = new Point2D(point.X + totalDist * Math.Cos(avgAngle), point.Y + totalDist * Math.Sin(avgAngle));

                Point2D newPointOrig = new Point2D(point.X + totalDist * Math.Cos(avgAngle), point.Y + totalDist * Math.Sin(avgAngle));

                if (direction == 1)
                {
                    //offset inside
                    if (WindingNumberInside(newPoint, polygon) != 0)
                    {
                        //if the new point is inside, then the new point has been offset inside the original shape
                    }
                    else
                    {
                        //The point was accidentally offset to the outside, so flip the average angle (subtract 180 degrees, or PI in this case)
                        avgAngle = avgAngle - Math.PI;
                        newPoint = new Point2D(point.X + totalDist * Math.Cos(avgAngle), point.Y + totalDist * Math.Sin(avgAngle));
                    }
                }
                else if (direction == -1)
                {
                    //offset outside
                    if (WindingNumberInside(newPoint, polygon) == 0)
                    {
                        //If the new point is outside, then the new point was offset outside correctly
                    }
                    else
                    {
                        //The point was accidentally offset to the outside, so flip the average angle (subtract 180 degrees, or PI in this case)
                        avgAngle = avgAngle - Math.PI;
                        newPoint = new Point2D(point.X + totalDist * Math.Cos(avgAngle), point.Y + totalDist * Math.Sin(avgAngle));
                    }
                }

                offsetPointList.Add(newPoint);
            }

            List<LineSegment2D> edgeList = new List<LineSegment2D>();

            for (int i = 0; i < offsetPointList.Count; i++)
            {
                Point2D point;
                Point2D nextPoint;

                int index = 0;
                int nextIndex = 0;

                if (i >= 0 && i < offsetPointList.Count - 1)
                {
                    index = i;
                    nextIndex = i + 1;
                }
                else
                {
                    index = i;
                    nextIndex = 0;
                }

                point = offsetPointList[index];
                nextPoint = offsetPointList[nextIndex];

                Point2D origPoint = pointList[index];
                Point2D origNextPoint = pointList[nextIndex];

                Vector2D unitVectorOrig = new Vector2D(origNextPoint.X - origPoint.X, origNextPoint.Y - origPoint.Y);
                double vectorLength = unitVectorOrig.Length;
                unitVectorOrig = new Vector2D(unitVectorOrig.X / vectorLength, unitVectorOrig.Y / vectorLength);

                Vector2D unitVectorOffset = new Vector2D(nextPoint.X - point.X, nextPoint.Y - point.Y);
                double vectorLengthOffset = unitVectorOffset.Length;
                unitVectorOffset = new Vector2D(unitVectorOffset.X / vectorLengthOffset, unitVectorOffset.Y / vectorLengthOffset);

                if (Math.Sign(Math.Round(unitVectorOrig.X, 4)) == Math.Sign(Math.Round(unitVectorOffset.X, 4)) && Math.Sign(Math.Round(unitVectorOrig.Y, 4)) == Math.Sign(Math.Round(unitVectorOffset.Y, 4)))
                {
                    edgeList.Add(new LineSegment2D(point, nextPoint));
                }
            }

            for (int i = 0; i < edgeList.Count; i++)
            {
                LineSegment2D nextEdge;
                LineSegment2D prevEdge;

                int prevEdgeIndex = 0;
                int nextEdgeIndex = 0;

                if (i == 0)
                {
                    prevEdge = edgeList[edgeList.Count - 1];
                    nextEdge = edgeList[i + 1];

                    prevEdgeIndex = edgeList.Count - 1;
                    nextEdgeIndex = i + 1;
                }
                else if (i == edgeList.Count - 1)
                {
                    prevEdge = edgeList[i - 1];
                    nextEdge = edgeList[0];

                    prevEdgeIndex = i - 1;
                    nextEdgeIndex = 0;
                }
                else
                {
                    prevEdge = edgeList[i - 1];
                    nextEdge = edgeList[i + 1];

                    prevEdgeIndex = i - 1;
                    nextEdgeIndex = i + 1;
                }

                if (edgeList[i].CheckForIntersection(nextEdge, out Point2D? intersectionNext) == true)
                {
                    if (edgeList[i].StartPoint.DistanceTo(intersectionNext.Value) <= 0.001)
                    {
                        //The intersection is the first point of the current line, this shouldn't happen
                        throw new Exception("Line intersection of the current edge and the next edge was found at the first point of the current edge.");
                    }
                    else if (edgeList[i].EndPoint.DistanceTo(intersectionNext.Value) <= 0.001)
                    {
                        //This means we are good, do nothing
                    }
                    else
                    {
                        //the intersection is somewhere in the line
                        edgeList[i] = new LineSegment2D(edgeList[i].StartPoint, intersectionNext.Value);
                        edgeList[nextEdgeIndex] = new LineSegment2D(intersectionNext.Value, edgeList[nextEdgeIndex].EndPoint);
                    }
                }

                if (edgeList[i].CheckForIntersection(prevEdge, out Point2D? intersectionPrev) == true)
                {
                    if (edgeList[i].StartPoint.DistanceTo(intersectionPrev.Value) <= 0.001)
                    {
                        //This means we are good, do nothing
                    }
                    else if (edgeList[i].EndPoint.DistanceTo(intersectionPrev.Value) <= 0.001)
                    {
                        //The intersection is the first point of the current line, this shouldn't happen
                        throw new Exception("Line intersection of the current edge and the next edge was found at the first point of the current edge.");
                    }
                    else
                    {
                        //the intersection is somewhere in the line
                        edgeList[i] = new LineSegment2D(intersectionNext.Value, edgeList[i].EndPoint);
                        edgeList[prevEdgeIndex] = new LineSegment2D(edgeList[nextEdgeIndex].StartPoint, intersectionNext.Value);
                    }
                }
            }

            List<Point2D> checkedOffsetPolygon = new List<Point2D>();

            foreach (LineSegment2D seg in edgeList)
            {
                checkedOffsetPolygon.Add(seg.StartPoint);
            }

            Polygon2D offsetPolygon = new Polygon2D(checkedOffsetPolygon);

            return offsetPolygon;
        }

        public static List<Angle> InteriorAngles(this Polygon2D boundary)
        {
            var cornerAngles = new List<Angle>();

            List<Vector2D> edgeVectors = boundary.Edges.Select(e => e.ToVector2D()).ToList();
            
            for (int i = 0; i < edgeVectors.Count; i++)
            {
                if (i != edgeVectors.Count - 1)
                {
                    cornerAngles.Add(Angle.FromDegrees(180) - edgeVectors[i].AngleTo(edgeVectors[i + 1]));
                }
                else
                {
                    cornerAngles.Add(Angle.FromDegrees(180) - edgeVectors[i].AngleTo(edgeVectors[0]));
                }
            }

            return cornerAngles;
        }

        public static bool IsRectangle(this Polygon2D boundary, double angleToleranceInDegrees = 1)
        {
            if (boundary.Vertices.Count() != 4) return false;
            
            List<double> cornerAngles = boundary.InteriorAngles().Select(a => a.Degrees).ToList();

            List<double> rightAngles =
                cornerAngles.Where(a => a <= 90 + angleToleranceInDegrees && a >= 90 - angleToleranceInDegrees).ToList();

            return cornerAngles.Count == rightAngles.Count;
        }

        public static LineSegment2D ShortestEdge(this Polygon2D boundary)
        {
            var edges = boundary.Edges.ToList();

            double shortestLength = edges[0].Length;
            LineSegment2D shortestEdge = edges[0];

            foreach (var edge in boundary.Edges)
            {
                var length = edge.Length;
                
                if (length < shortestLength)
                {
                    shortestLength = length;
                    shortestEdge = edge;
                } 
            }

            return shortestEdge;
        }

        public static LineSegment2D LongestEdge(this Polygon2D boundary)
        {
            var edges = boundary.Edges.ToList();

            double longestLength = edges[0].Length;
            LineSegment2D longestEdge = edges[0];

            foreach (var edge in boundary.Edges)
            {
                var length = edge.Length;
                
                if (length > longestLength)
                {
                    longestLength = length;
                    longestEdge = edge;
                } 
            }

            return longestEdge;
        }
        
        public static MinMaxBounds GetMinMaxBounds(this Polygon2D pg)
        {
            List<Point2D> points = pg.Vertices.ToList();

            double maxX = points[0].X;
            double minX = points[0].X;
            double maxY = points[0].Y;
            double minY = points[0].Y;

            for (var i = 1; i < points.Count; i++)
            {
                Point2D p = points[i];
                
                if (p.X < minX)
                {
                    minX = p.X;
                }
                if (p.X > maxX)
                {
                    maxX = p.X;
                }
                if (p.Y < minY)
                {
                    minY = p.Y;
                }
                if (p.Y > maxY)
                {
                    maxY = p.Y;
                }
            }

            return new MinMaxBounds { MaxX = maxX, MinX = minX, MaxY = maxY, MinY = minY };
        }
    }
}