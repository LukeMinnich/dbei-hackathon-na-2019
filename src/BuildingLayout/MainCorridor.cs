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

        public void CalculateMainLines()
        {
            var lineVector = new Vector2D();
            var moveVector = new Vector2D(0, HallWidth);

            var lineLeft = CenterLine + moveVector;
            var lineRight = CenterLine - moveVector;

            LineLeft = new List<UnitsLine>();
            LineRight = new UnitsLine(lineRight);

            var corridorWidth = (UnitDepth * 2 + HallWidth);

            switch (CorridorLocation)
            {
                case CorridorLocation.Main:
                    if (LeftLeg != null)
                    {
                        lineVector = CenterLine.Direction / CenterLine.Length;
                        lineLeft = new Line2D(lineLeft.StartPoint + lineVector * corridorWidth, lineLeft.EndPoint);
                    }
                    if (RightLeg != null)
                    {
                        lineVector = CenterLine.Direction / CenterLine.Length;
                        lineLeft = new Line2D(lineLeft.StartPoint, lineLeft.EndPoint - lineVector * corridorWidth);
                    }
                    if (MiddleLeg != null)
                    {
                        lineVector = CenterLine.Direction / CenterLine.Length;
                        var lineLeft1 = new Line2D(lineLeft.StartPoint, MiddleLeg.CenterLine.StartPoint - lineVector * corridorWidth / 2);
                        var lineLeft2 = new Line2D(MiddleLeg.CenterLine.StartPoint, lineLeft.StartPoint - lineVector * corridorWidth / 2);
                        LineLeft.Add(new UnitsLine(lineLeft1));
                        LineLeft.Add(new UnitsLine(lineLeft2));
                    }
                    else if(MiddleLeg == null)
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
