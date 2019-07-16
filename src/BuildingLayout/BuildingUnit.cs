using System;

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
