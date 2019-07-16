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
    }
}