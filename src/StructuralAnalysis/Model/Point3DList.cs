using System;
using System.Collections.Generic;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class Point3DList : List<Point3D>
    {
        private const double PLANEROTATIONTOLERANCE = 0.000001;

        public Point3DList(IEnumerable<Point3D> points) : base(points)
        {
        }

        public Point3DList Translate(Vector3D vector)
        {
            var newPoints = new LinkedList<Point3D>();

            foreach (var point in this)
            {
                newPoints.AddLast(point + vector);
            }

            return new Point3DList(newPoints);
        }

        public Point3DList RotateToAxis(Angle rotation, UnitVector3D axis)
        {
            if (Math.Abs(rotation.Radians) > PLANEROTATIONTOLERANCE)
            {
                var newPoints = new LinkedList<Point3D>();
                
                foreach (var point in this)
                {
                    newPoints.AddLast(point.Rotate(axis, rotation));
                }
                
                return new Point3DList(newPoints);
            }

            return this;
        }
    }
}