using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingLayout
{
    public class MainCorridor : Corridor
    {
        public Corridor LeftLeg { get; set; }
        public Corridor MiddleLeg { get; set; }
        public Corridor RightLeg { get; set; }

        public MainCorridor()
        {

        }

        public void CalculateMainLines()
        {
            var lineVector = new Vector2D();
            var moveVector = new Vector2D(0, HallWidth/2);

            var lineLeft = CenterLine + moveVector;
            var lineRight = CenterLine - moveVector;

            LineLeft = new List<UnitsLine>();

            var lineRightSwapped = new Line2D(lineRight.EndPoint, lineRight.StartPoint);

            LineRight = new UnitsLine(lineRightSwapped);

            var corridorWidth = (UnitDepth * 2 + HallWidth);

            switch (CorridorLocation)
            {
                case CorridorLocation.Main:
                    if (LeftLeg.LineRight != null)
                    {
                        lineVector = CenterLine.Direction;
                        lineLeft = new Line2D(lineLeft.StartPoint + lineVector * corridorWidth, lineLeft.EndPoint);
                    }
                    if (RightLeg.LineRight != null)
                    {
                        lineVector = CenterLine.Direction;
                        lineLeft = new Line2D(lineLeft.StartPoint, lineLeft.EndPoint - lineVector * corridorWidth);
                    }
                    if (MiddleLeg.LineRight != null)
                    {
                        lineVector = CenterLine.Direction;
                        var lineLeft1 = new Line2D(lineLeft.StartPoint, MiddleLeg.CenterLine.StartPoint + moveVector - lineVector * corridorWidth / 2);
                        var lineLeft2 = new Line2D(MiddleLeg.CenterLine.StartPoint+moveVector + lineVector * corridorWidth / 2, lineLeft.EndPoint);
                        LineLeft.Add(new UnitsLine(lineLeft1));
                        LineLeft.Add(new UnitsLine(lineLeft2));
                    }
                    else if (MiddleLeg.LineRight == null)
                    {
                        LineLeft.Add(new UnitsLine(lineLeft));
                    }

                    break;

                default:
                    break;
            }
        }
    }
}
