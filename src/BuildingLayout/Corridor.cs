using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingLayout
{
    public class Corridor
    {
        public Line2D CenterLine { get; set; }
        public double UnitDepth { get; set; }
        public double HallWidth { get; set; }
        public CorridorLocation CorridorLocation { get; set; }
        public List<Line2D> LineLeft { get; set; }
        public Line2D LineRight { get; set; }

        public Corridor()
        {

        }

        public Corridor(Line2D line, double unitDepth, double hallWidth, CorridorLocation location)
        {
            CenterLine = line;
            UnitDepth = UnitDepth;
            HallWidth = hallWidth;
            CorridorLocation = location;
        }

        public void CalculateLegLines()
        {
            var lineLeft = new Line2D();
            var lineRight = new Line2D();
            var lineVector = new Vector2D();
            var moveVector = new Vector2D(HallWidth, 0);

            switch (CorridorLocation)
            {
                case CorridorLocation.LeftLeg:
                    lineVector = CenterLine.Direction / CenterLine.Length;
                    lineRight = new Line2D((CenterLine.StartPoint + lineVector * UnitDepth + moveVector), CenterLine.EndPoint + moveVector);
                    lineLeft = new Line2D((CenterLine.StartPoint - moveVector), CenterLine.EndPoint - moveVector);
                    break;
                case CorridorLocation.MiddleLeg:
                    lineVector = CenterLine.Direction / CenterLine.Length;
                    lineLeft = new Line2D((CenterLine.StartPoint + lineVector * UnitDepth - moveVector), CenterLine.EndPoint - moveVector);
                    lineRight = new Line2D((CenterLine.StartPoint + lineVector * UnitDepth + moveVector), CenterLine.EndPoint + moveVector);

                    break;
                case CorridorLocation.RightLeg:
                    lineVector = CenterLine.Direction / CenterLine.Length;
                    lineLeft = new Line2D((CenterLine.StartPoint + lineVector * UnitDepth - moveVector), CenterLine.EndPoint - moveVector);
                    lineRight = new Line2D((CenterLine.StartPoint + moveVector), CenterLine.EndPoint + moveVector);
                    break;
                default:
                    break;
            }

            LineLeft = new List<Line2D>() { lineLeft };
            LineRight = lineRight;

        }


    }
}
