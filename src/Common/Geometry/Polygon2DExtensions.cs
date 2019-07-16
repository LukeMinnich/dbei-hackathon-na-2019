using System.Collections.Generic;
using System.Linq;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.Common.Geometry
{
    public static class Polygon2DExtensions
    {
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

        private static Polygon2D ProjectToHorizontalPlane(IEnumerable<Point3D> vertices)
        {
            var vertices2D = new LinkedList<Point2D>();

            foreach (var vertex in vertices)
            {
                vertices2D.AddLast(new Point2D(vertex.X, vertex.Y));
            }

            return new Polygon2D(vertices2D);
        }
    }
}