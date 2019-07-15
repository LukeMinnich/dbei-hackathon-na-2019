using Kataclysm.Common.Extensions;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Kataclysm.Common
{
    public class PointLoad3D : Load3D
    {
        public readonly Point3D Location;
        public double Fx { get; private set; }
        public double Fy { get; private set; }
        public double Fz { get; private set; }
        public double Mx { get; private set; }
        public double My { get; private set; }
        public double Mz { get; private set; }

        public Vector3D Forces
        {
            get => new Vector3D(Fx, Fy, Fz);

            private set
            {
                Fx = value.X;
                Fy = value.Y;
                Fz = value.Z;
            }
        }

        public Vector3D Moments
        {
            get => new Vector3D(Mx, My, Mz);

            private set
            {
                Mx = value.X;
                My = value.Y;
                Mz = value.Z;
            }
        }

        public PointLoad3D(Point3D location, double fx, double fy, double fz, double mx, double my, double mz,
            LoadPattern loadPattern)
        {
            Location = location;
            Fx = fx;
            Fy = fy;
            Fz = fz;
            Mx = mx;
            My = my;
            Mz = mz;
            LoadPattern = loadPattern;
        }

        public PointLoad3D(double fx, double fy, double fz, double mx, double my, double mz)
        {
            Fx = fx;
            Fy = fy;
            Fz = fz;
            Mx = mx;
            My = my;
            Mz = mz;
        }

        public PointLoad3D(Point3D location, Vector3D forces, Vector3D moments, LoadPattern loadPattern)
        {
            Location = location;
            Forces = forces;
            Moments = moments;
            LoadPattern = loadPattern;
        }

        public PointLoad3D(Point3D location, Vector3D forces, LoadPattern loadPattern)
        {
            Location = location;
            Forces = forces;
            LoadPattern = loadPattern;
        }

        public PointLoad3D(Point3D location, LoadPattern loadPattern)
        {
            Location = location;
            LoadPattern = loadPattern;
        }

        public PointLoad3D(Vector3D forces, Vector3D moments)
        {
            Forces = forces;
            Moments = moments;
        }

        public bool IsZero()
        {
            return Fx == 0 && Fy == 0 && Fz == 0 && Mx == 0 && My == 0 && Mz == 0;
        }

        public PointLoad3D RotateAboutZAxisAndCenter(Angle angle, Point2D center)
        {
            var newPoint = Location.RotateAroundZAxis(angle, center);

            var newForces = Forces.Rotate(UnitVector3D.ZAxis, angle);

            var newMoments = Moments.Rotate(UnitVector3D.ZAxis, angle);

            return new PointLoad3D(newPoint, newForces, newMoments, LoadPattern);
        }

        public PointLoad3D Translate(Vector3D vector)
        {
            return new PointLoad3D(Location + vector, Forces, Moments, LoadPattern);
        }

        public override string ToString()
        {
            return $"point load at {Location}";
        }

        public override double TotalVerticalForce => Fz;
    }
}