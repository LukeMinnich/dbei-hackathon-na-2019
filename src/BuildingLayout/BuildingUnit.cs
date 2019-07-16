using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;

namespace BuildingLayout
{
    public class BuildingUnit
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double Width { get; set; }
        public double Depth { get; set; }
        public double DoorCenterLocation { get; set; }
        public double DoorWidth { get; set; }

        public PolyLine2D GetPolyline()
        {
            var point1 = new Point2D(0,0);
            var point2 = new Point2D(0, Depth);
            var point3 = new Point2D(Width, Depth);
            var point4 = new Point2D(Width, 0);
            var point5 = point1;

            var pointList = new List<Point2D>()
            {
                point1,
                point2,
                point3,
                point4,
                point5
            };

            return new PolyLine2D(pointList);            
        }
        
        public double Area()
        {
            return Width * Depth;
        }

        public BuildingUnit(string name, string type, double width, double depth, double doorCenter, double doorWidth)
        {
            Name = name;
            Type = type;
            Width = width;
            Depth = depth;
            DoorCenterLocation = doorCenter;
            DoorWidth = doorWidth;
        }


    }
}
