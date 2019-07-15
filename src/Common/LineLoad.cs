using System;
using System.Collections.Generic;
using Kataclysm.Common.Extensions;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.Common
{
    public abstract class LineLoad : FloorLoad
    {
        public LineSegment2D Line { get; set; }

        public double MagnitudeStart { get; set; }

        public double MagnitudeEnd { get; set; }

        public LineLoad(LineSegment2D line, double startMagnitude, double endMagnitude, BuildingLevel level, Projection projection, LoadPattern loadPattern):base(level, projection,loadPattern)
        {
            Line = line;
            MagnitudeStart = startMagnitude;
            MagnitudeEnd = endMagnitude;
        }

        public LineLoad(LineSegment2D line, double startMagnitude, double endMagnitude) : base()
        {
            Line = line;
            MagnitudeStart = startMagnitude;
            MagnitudeEnd = endMagnitude;
        }

        public LineLoad()
        {

        }

        public override Point2D LoadCentroid()
        {
            double magnitude1 = 0;
            double centroid1 = 0;
            double magnitude2 = 0;
            double centroid2 = 0;

            if (Math.Sign(MagnitudeStart) == Math.Sign(MagnitudeEnd) || MagnitudeEnd * MagnitudeStart == 0)
            {
                //If the signs of the loads are the same OR one of the loads is 0
                magnitude1 = Math.Sign(MagnitudeStart) * Math.Min(Math.Abs(MagnitudeStart), Math.Abs(MagnitudeEnd)) * ((Line.EndPoint - Line.StartPoint).Length);
                centroid1 = ((Line.EndPoint - Line.StartPoint).Length) / 2.0;
                magnitude2 = Math.Sign(MagnitudeStart) * (Math.Max(Math.Abs(MagnitudeStart), Math.Abs(MagnitudeEnd)) - Math.Min(Math.Abs(MagnitudeStart), Math.Abs(MagnitudeEnd))) * ((Line.EndPoint - Line.StartPoint).Length) * 0.5;
                if (Math.Abs(MagnitudeStart) > Math.Abs(MagnitudeEnd))
                {
                    centroid2 = (1.0 / 3.0) * ((Line.EndPoint - Line.StartPoint).Length);
                }
                else
                {
                    centroid2 = (2.0 / 3.0) * ((Line.EndPoint - Line.StartPoint).Length);
                }
                double centroidLength = (Math.Abs(magnitude1) * centroid1 + Math.Abs(magnitude2) * centroid2) / (Math.Abs(magnitude1) + Math.Abs(magnitude2));

                Vector2D uv = Line.CalculateUnitVector();
                Vector2D centroidLengthVector = uv * centroidLength;
                Point2D centroid = new Point2D(Line.StartPoint.X + centroidLengthVector.X, Line.StartPoint.Y + centroidLengthVector.Y);
                return centroid;
            }

            if (Math.Sign(MagnitudeStart) != Math.Sign(MagnitudeEnd))
            {
                //The loads have different signs (i.e. one is negative and the other positive)
                double slope = (MagnitudeEnd - MagnitudeStart) / ((Line.EndPoint - Line.StartPoint).Length);
                double interceptLen = -(MagnitudeStart) / slope;
                Vector2D uv = Line.CalculateUnitVector();
                Vector2D interceptVector = uv * interceptLen;
                Point2D intercept = new Point2D(Line.StartPoint.X + interceptVector.X, Line.StartPoint.Y + interceptVector.Y);

                magnitude1 = (MagnitudeStart) * ((intercept - Line.StartPoint).Length) * 0.5; //The load coming from the starting triangle
                centroid1 = (1.0 / 3.0) * ((intercept - Line.StartPoint).Length);
                magnitude2 = (MagnitudeEnd) * ((Line.EndPoint - intercept).Length) * 0.5; //The load coming from the ending triangle
                centroid2 = interceptLen + (2.0 / 3.0) * ((Line.EndPoint - intercept).Length);
                double centroidLength = (Math.Abs(magnitude1) * centroid1 + Math.Abs(magnitude2) * centroid2) / (Math.Abs(magnitude1) + Math.Abs(magnitude2));

                Vector2D centroidLengthVector = uv * centroidLength;
                Point2D centroid = new Point2D(Line.StartPoint.X + centroidLengthVector.X, Line.StartPoint.Y + centroidLengthVector.Y);
                return centroid;
            }

            throw new Exception("The magnitude of the load could not be calculated");
        }

        public override double LoadMagnitude()
        {
            double magnitude = 0;

            //The start location is less than the end location
            if (MagnitudeStart >= 0 && MagnitudeEnd >= 0)
            {
                //The loads are both positive (or one is positive and the other is zero)
                magnitude += Math.Min(MagnitudeStart, MagnitudeEnd) * ((Line.EndPoint - Line.StartPoint).Length); //Uniform loading portion
                magnitude += (Math.Max(MagnitudeStart, MagnitudeEnd) - Math.Min(MagnitudeStart, MagnitudeEnd)) * ((Line.EndPoint - Line.StartPoint).Length) * 0.5; //triangular loading portion
                return magnitude;
            }

            if (MagnitudeStart <= 0 && MagnitudeEnd <= 0)
            {
                //The loads are both negative (or one is negative and the other is zero)
                magnitude += Math.Max(MagnitudeStart, MagnitudeEnd) * ((Line.EndPoint - Line.StartPoint).Length); //Uniform loading portion
                magnitude += (Math.Min(MagnitudeStart, MagnitudeEnd) - Math.Max(MagnitudeStart, MagnitudeEnd)) * ((Line.EndPoint - Line.StartPoint).Length) * 0.5; //triangular loading portion
                return magnitude;
            }

            if (Math.Sign(MagnitudeStart) != Math.Sign(MagnitudeEnd))
            {
                //The loads have different signs (i.e. one is negative and the other positive)
                double slope = (MagnitudeEnd - MagnitudeStart) / ((Line.EndPoint - Line.StartPoint).Length);
                double interceptLen = -(MagnitudeStart) / slope;
                Vector2D uv = Line.CalculateUnitVector();
                Vector2D interceptVector = uv * interceptLen;
                Point2D intercept = new Point2D(Line.StartPoint.X + interceptVector.X, Line.StartPoint.Y + interceptVector.Y);

                magnitude += (MagnitudeStart) * ((intercept - Line.StartPoint).Length) * 0.5; //The load coming from the starting triangle
                magnitude += (MagnitudeEnd) * ((Line.EndPoint - intercept).Length) * 0.5; //The load coming from the ending triangle
                return magnitude;
            }

            throw new Exception("The magnitude of the load could not be calculated");
        }

        public override double VerticalLoadMagnitudeProjected(Plane plane, out Point2D loadCentroid)
        {
            double magnitude = 0;

            if (Projection.Equals(Projection.AlongSlope) == true)
            {
                magnitude = GetVerticalLoadMagnitudeProjectedAlongSlope(plane, MagnitudeStart, MagnitudeEnd, out Point2D centroid);

                loadCentroid = centroid;
            }
            else if (Projection.Equals(Projection.Vertical) == true)
            {
                Plane horizPlane = Plane.FromPoints(new Point3D(0, 0, 0), new Point3D(0, 10, 0), new Point3D(10, 10, 0));

                magnitude = GetVerticalLoadMagnitudeProjectedAlongSlope(horizPlane, MagnitudeStart, MagnitudeEnd, out Point2D centroid);

                loadCentroid = centroid;
            }
            else
            {
                //The control vertex magnitudes are the magnitudes normal to the projected plane.
                //First, find the global Z component of the control vertex magnitudes

                double rotatedStartMagnitude = GetRotatedLoadMagnitude(plane, MagnitudeStart);
                double rotatedEndMagnitude = GetRotatedLoadMagnitude(plane, MagnitudeEnd);

                magnitude = GetVerticalLoadMagnitudeProjectedAlongSlope(plane, rotatedStartMagnitude, rotatedEndMagnitude, out Point2D centroid);

                loadCentroid = centroid;
            }

            return magnitude;
        }

        private double GetRotatedLoadMagnitude(Plane plane, double load)
        {
            Point3D startPointOnPlane = Line.StartPoint.MapToPlane(plane);
            Point3D endPointOnPlane = Line.EndPoint.MapToPlane(plane);

            Vector3D localX = new Vector3D(endPointOnPlane.X - startPointOnPlane.X, endPointOnPlane.Y - startPointOnPlane.Y, endPointOnPlane.Z - startPointOnPlane.Z);

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