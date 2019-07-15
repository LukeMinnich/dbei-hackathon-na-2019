using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

            // Check for simple polygon valid only after planar polygon is confirmed
            CheckForSimplePolygon();
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

        private void CheckForSimplePolygon()
        {
            if (!IsSimplePolygon())
            {
                throw new SimplePolygonException("Polygon must be simple. " +
                                                 "No polygon edges can intersect");
            }
        }

        private bool IsSimplePolygon()
        {
            return IsTriangularPolygon() ||
                   IsSimplePolygonWithFourOrMoreSides();
        }

        private bool IsTriangularPolygon()
        {
            return _vertices.Count == 3;
        }

        private bool IsSimplePolygonWithFourOrMoreSides()
        {
            if (_vertices.Count < 4)
            {
                return false;
            }

            var transformedVertices = RotateToHorizontalPlane();

            try
            {
                var flattenedPolygon = ProjectToHorizontalPlane(transformedVertices);
                _area = flattenedPolygon.GetArea();
                return true;
            }
            catch (SimplePolygonException)
            {
                return false;
            }
        }

        public IReadOnlyList<Point3D> RotateToHorizontalPlane()
        {
            var vertices = new Point3DList(_vertices);

            // Translation to cartesian origin
            var transformedVertices =
                vertices.Translate(new Vector3D(-1 * _vertices[0].X, -1 * _vertices[0].Y, -1 * _vertices[0].Z));

            // Rotations about X
            var rotX = -1 * GetInclinedRotationsForXAndY(transformedVertices).AboutXAxis;

            transformedVertices = transformedVertices.RotateToAxis(rotX, UnitVector3D.XAxis);

            // Rotations about Y using new rotations
            var rotY = -1 * GetInclinedRotationsForXAndY(transformedVertices).AboutYAxis;

            transformedVertices = transformedVertices.RotateToAxis(rotY, UnitVector3D.YAxis);

            // Reverse translation to cartesian origin
            transformedVertices =
                transformedVertices.Translate(new Vector3D(_vertices[0].X, _vertices[0].Y, _vertices[0].Z));

            return new ReadOnlyCollection<Point3D>(transformedVertices.ToList());
        }

        private static SimplePolygon2D ProjectToHorizontalPlane(IEnumerable<Point3D> vertices)
        {
            var vertices2D = new LinkedList<Point2D>();

            foreach (var vertex in vertices)
            {
                vertices2D.AddLast(new Point2D(vertex.X, vertex.Y));
            }

            return new SimplePolygon2D(vertices2D);
        }

        public SimplePolygon2D ProjectToHorizontalPlane()
        {
            return ProjectToHorizontalPlane(_vertices);
        }

        private static InclinedRotations GetInclinedRotationsForXAndY(Point3DList vertices)
        {
            var plane = Plane.FromPoints(vertices[0], vertices[1], vertices[2]);

            return plane.GetInclinedRotations();
        }

        public InclinedRotations GetInclinedRotationsForXAndY()
        {
            return GetInclinedRotationsForXAndY(_vertices);
        }

        public IReadOnlyList<Point3D> Vertices()
        {
            return new ReadOnlyCollection<Point3D>(_vertices.ToArray());
        }

        public double GetArea()
        {
            return _area;
        }

        public bool IsConvex()
        {
            if (_vertices.Count < 4)
            {
                return true;
            }

            var transformedVertices = RotateToHorizontalPlane();
            var flattenedPolygon = ProjectToHorizontalPlane(transformedVertices);

            return flattenedPolygon.IsConvex();
        }

        public bool IsProjectionClockwise()
        {
            var transformedVertices = RotateToHorizontalPlane();
            return (ProjectToHorizontalPlane(transformedVertices).IsClockwise());
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