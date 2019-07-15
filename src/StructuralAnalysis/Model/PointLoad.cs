using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class  PointLoad : FloorLoad
    {
        public Point2D? Location { get; set; }
        public double? Magnitude { get; set; }

        public PointLoad(Point2D location, double magnitude, BuildingLevel level, Projection projection, LoadPattern loadPattern) : base(level, projection, loadPattern)
        {
            Location = location;
            Magnitude = magnitude;
        }

        public PointLoad(Point2D location, double magnitude) : base()
        {
            Location = location;
            Magnitude = magnitude;
        }

        public PointLoad()
        {

        }

        public override Point2D LoadCentroid()
        {
            return Location.Value;
        }

        public override double LoadMagnitude()
        {
            return Magnitude.Value;
        }

        public override double VerticalLoadMagnitudeProjected(Plane plane, out Point2D loadCentroid)
        {
            double magnitude = 0;

            if (Projection.Equals(Projection.AlongSlope) == true)
            {
                magnitude = Magnitude.Value;

                loadCentroid = Location.Value;
            }
            else if (Projection.Equals(Projection.Vertical) == true)
            {
                magnitude = Magnitude.Value;

                loadCentroid = Location.Value;
            }
            else
            {
                //The control vertex magnitudes are the magnitudes normal to the projected plane.
                //First, find the global Z component of the control vertex magnitudes

                magnitude = GetRotatedLoadMagnitude(plane, Magnitude.Value);

                loadCentroid = Location.Value;
            }

            return magnitude;
        }

        private double GetRotatedLoadMagnitude(Plane plane, double load)
        {
            Point3D pointOnPlane = Location.Value.MapToPlane(plane);

            Point3D arbPointOnPlane = (Location.Value + new Point2D(1, 0)).MapToPlane(plane); //arbitrary point

            Vector3D localX = new Vector3D(arbPointOnPlane.X - pointOnPlane.X, arbPointOnPlane.Y - pointOnPlane.Y, arbPointOnPlane.Z - pointOnPlane.Z);

            UnitVector3D localZ = plane.Normal;

            //Ensure that the local Z axis is not pointing downwards (otherwise we get opposite sign)
            if (localZ.Z < 0)
            {
                localZ = new UnitVector3D(-localZ.X, -localZ.Y, -localZ.Z);
            }

            double[,] RotMatrix = BuildRotationMatrix(localZ, localX, 0);

            Vector3D localLoadVector = new Vector3D(0, 0, load);

            double[,] globalLoadVector = MatrixVectorMultiply(RotMatrix, localLoadVector);

            double rotatedVerticalMagnitude = globalLoadVector[2, 0];

            return rotatedVerticalMagnitude;

        }
    }
}