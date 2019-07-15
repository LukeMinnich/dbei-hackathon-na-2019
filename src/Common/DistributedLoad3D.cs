using System;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Kataclysm.Common
{
    public class DistributedLoad3D : Load3D
    {
        public PointLoad3D StartLoad { get; protected set; }
        public PointLoad3D EndLoad { get; protected set; }

        public DistributedLoad3D(PointLoad3D startLoad, PointLoad3D endLoad)
        {
            CheckLoads(startLoad, endLoad);

            StartLoad = startLoad;
            EndLoad = endLoad;

            LoadPattern = startLoad.LoadPattern;
        }

        private void CheckLoads(PointLoad3D PL1, PointLoad3D PL2)
        {
            if (PL1.LoadPattern.Equals(PL2.LoadPattern) == false)
            {
                throw new ArgumentException("Distributed load start and end loads must be of the same load pattern.");
            }
        }

        public DistributedLoad3D RotateAroundZAxisAtCenter(Angle rotation, Point2D center)
        {
            return new DistributedLoad3D(StartLoad.RotateAboutZAxisAndCenter(rotation, center),
                EndLoad.RotateAboutZAxisAndCenter(rotation, center));
        }

        public DistributedLoad3D Translate(Vector3D vector)
        {
            return new DistributedLoad3D(StartLoad.Translate(vector), EndLoad.Translate(vector));
        }

        public override string ToString()
        {
            return $"distributed load from {StartLoad.Location} to {EndLoad.Location}";
        }

//        public PointLoad3D LinearInterpolationAtPoint(Point3D point)
//        {
//            var interpolatedForces = point.InterpolateVector(StartLoad.Forces, EndLoad.Forces,
//                StartLoad.Location, EndLoad.Location);
//
//            var interpolatedMoments = point.InterpolateVector(StartLoad.Moments, EndLoad.Moments,
//                StartLoad.Location, EndLoad.Location);
//
//            return new PointLoad3D(point, interpolatedForces, interpolatedMoments, StartLoad.LoadPattern);
//        }

        public override double TotalVerticalForce
        {
            get
            {
                var startTotal = StartLoad.TotalVerticalForce;
                var endTotal = EndLoad.TotalVerticalForce;

                var loadLength = StartLoad.Location.DistanceTo(EndLoad.Location);

                return (startTotal + endTotal) / 2 * loadLength;
            }
        }
    }
}