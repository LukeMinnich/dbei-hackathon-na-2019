using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Kataclysm.Common.Extensions;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("Katerra.Apollo.Structures.IO.Tests")]

namespace Kataclysm.Common.Geometry
{
    public class Polygon3D
    {
        #region Fields
        
        [JsonProperty]
        private Point3DList _vertices;

        [JsonProperty]
        private double _area;
        
        private const double POINTTOPLANEDISTANCETOLERANCE = 0.001;

        #endregion

        #region Constructors 

        public Polygon3D(IEnumerable<Point3D> vertices)
        {
            if (vertices == null)
            {
                return;
            }
            _vertices = new Point3DList(vertices);

            CheckForMinimum3Vertices();
            CheckIfPolygonIsPlanar();
        }

        #endregion

        #region API Functions

        public static Polygon3D FromVertices(IEnumerable<Point3D> vertices)
        {
            return new Polygon3D(vertices);
        }

        public static Polygon3D FromPlaneAndPolygon2D(Plane plane, Polygon2D boundary)
        {
            var vertices2D = boundary.Vertices;
            var vertices3D = new LinkedList<Point3D>();

            foreach (var vertex in vertices2D)
            {
                vertices3D.AddLast(vertex.MapToPlane(plane));
            }

            return FromVertices(vertices3D);
        }

        #endregion

        private void CheckForMinimum3Vertices()
        {
            if (_vertices.Count < 3)
            {
                throw new InvalidDataException("Polygon must have a minimum of 2 vertices.");
            }
        }

        private void CheckIfPolygonIsPlanar()
        {
            if (!IsPlanar())
            {
                throw new InvalidDataException("Polygon must be comprised of planar vertices.");
            }
        }

        private bool IsPlanar()
        {
            var basePlane = Plane.FromPoints(_vertices[0], _vertices[1], _vertices[2]);

            for (var i = 3; i < _vertices.Count; i++)
            {
                var distanceBetweenVertexAndPlane = basePlane.AbsoluteDistanceTo(_vertices[i]);
                if (Math.Abs(distanceBetweenVertexAndPlane) > POINTTOPLANEDISTANCETOLERANCE)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsTriangularPolygon()
        {
            return _vertices.Count == 3;
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

        public Polygon2D ProjectToHorizontalPlane()
        {
            return ProjectToHorizontalPlane(_vertices);
        }

        public IReadOnlyList<Point3D> Vertices()
        {
            return new ReadOnlyCollection<Point3D>(_vertices.ToArray());
        }

        public double GetArea()
        {
            return _area;
        }

        public Polygon3D Reverse()
        {
            var vertices = (IEnumerable<Point3D>) _vertices;
            return new Polygon3D(vertices.Reverse().ToList());
        }

        public Polygon3D Rotate(Angle rotation, Vector3D aboutVector)
        {
            var rotatedVertices = new LinkedList<Point3D>();

            foreach (var vertex in _vertices)
            {
                rotatedVertices.AddLast(vertex.Rotate(aboutVector, rotation));
            }

            return new Polygon3D(rotatedVertices);
        }

        public Point2D Get2Dcentroid()
        {
            // return the centroid of the horizontal projection of the polygon
            double sumx = 0;
            double sumy = 0;
            foreach (Point3D pt in _vertices)
            {
                sumx += pt.X;
                sumy += pt.Y;
            }
            Point2D Centroid2D = new Point2D(sumx / _vertices.Count, sumy / _vertices.Count);
            return Centroid2D;
        }
    }
}