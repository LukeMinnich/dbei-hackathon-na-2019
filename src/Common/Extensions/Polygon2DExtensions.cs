using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.Common.Extensions
{
    public static class Polygon2DExtensions
    {
        public static Tuple<Point2D, Point2D> GetMinMaxPointsAtGreatestBoundaryExtent(this Polygon2D boundary,
            LateralDirection direction)
        {
            List<Point2D> points = boundary.Vertices.ToList();

            var minPoint = points[0];
            var maxPoint = points[0];

            for (var i = 1; i < points.Count; i++)
            {
                var point = points[i];
                
                switch (direction)
                {
                    case LateralDirection.X:
                        if (point.X < minPoint.X) minPoint = point;
                        if (point.X > maxPoint.X) maxPoint = point;
                        break;
                    case LateralDirection.Y:
                        if (point.Y < minPoint.Y) minPoint = point;
                        if (point.Y > maxPoint.Y) maxPoint = point;
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }
            
            return new Tuple<Point2D, Point2D>(minPoint, maxPoint);
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
        
        // Per https://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
        public static double GetArea(this Polygon2D polygon)
        {
            return Math.Abs(GetSignedArea(polygon));
        }
        
        // Per https://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
        private static double GetSignedArea(this Polygon2D polygon)
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

            double area = polygon.GetSignedArea();

            double cx = 1.0 / (6.0 * area) * sumX;

            double cy = 1.0 / (6.0 * area) * sumY;

            return new Point2D(cx, cy);
        }
    }
}