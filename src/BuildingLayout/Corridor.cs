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
        public List<UnitsLine> LineLeft { get; set; }
        public UnitsLine LineRight { get; set; }

        public Corridor()
        {

        }


        public Corridor(Line2D line, double unitDepth, double hallWidth, CorridorLocation location)
        {
            CenterLine = line;
            UnitDepth = unitDepth;
            HallWidth = hallWidth;
            CorridorLocation = location;

            CalculateLegLines();
        }

        public List<UnitsLine> GetLines()
        {
            List<UnitsLine> lineList = new List<UnitsLine>();
            lineList.AddRange(LineLeft);
            lineList.Add(LineRight);

            return lineList;
        }

        public void CalculateLegLines()
        {
            var lineLeft = new Line2D();
            var lineRight = new Line2D();
            var lineVector = CenterLine.Direction;
            var moveVector = new Vector2D(HallWidth/2, 0);

            switch (CorridorLocation)
            {
                case CorridorLocation.LeftLeg:
                    lineVector = CenterLine.Direction;
                    lineRight = new Line2D((CenterLine.StartPoint + lineVector * UnitDepth + moveVector), CenterLine.EndPoint + moveVector);
                    lineLeft = new Line2D((CenterLine.StartPoint - moveVector), CenterLine.EndPoint - moveVector);
                    break;
                case CorridorLocation.MiddleLeg:
                    lineVector = CenterLine.Direction;
                    lineLeft = new Line2D((CenterLine.StartPoint + lineVector * UnitDepth - moveVector), CenterLine.EndPoint - moveVector);
                    lineRight = new Line2D((CenterLine.StartPoint + lineVector * UnitDepth + moveVector), CenterLine.EndPoint + moveVector);

                    break;
                case CorridorLocation.RightLeg:
                    lineVector = CenterLine.Direction;
                    lineLeft = new Line2D((CenterLine.StartPoint + lineVector * UnitDepth - moveVector), CenterLine.EndPoint - moveVector);
                    lineRight = new Line2D((CenterLine.StartPoint + moveVector), CenterLine.EndPoint + moveVector);
                    break;
                default:
                    break;
            }

            LineLeft = new List<UnitsLine>() { new UnitsLine(lineLeft) };

            var lineRightSwapped = new Line2D(lineRight.EndPoint, lineRight.StartPoint);

            LineRight = new UnitsLine(lineRightSwapped);

        }


    }
}
