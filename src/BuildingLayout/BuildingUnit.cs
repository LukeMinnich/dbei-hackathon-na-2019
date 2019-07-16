using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public Point2D Location { get; set; }
        public Angle Rotation { get; set; }    

        public Polygon2D GetPolygon()
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

            var polyLine2D = new PolyLine2D(pointList);
            var moveLine = new Line2D(point1, Location);
            var moveVector = moveLine.Direction*moveLine.Length;
            var polygon = new Polygon2D(pointList);

            polygon = polygon.RotateAround(Rotation, point1);
            polygon = polygon.TranslateBy(moveVector);
            
            return polygon;          
        }

        public List<LineSegment2D> GetShearWallLines()
        {
            var polygon = GetPolygon();

            var edges = polygon.Edges.ToList();

            var moveVector1 = edges[0].Direction;
            var moveVector2 = edges[2].Direction;

            var shearWall1 = edges[1].TranslateBy(moveVector2*1);
            var shearWall2 = edges[3].TranslateBy(moveVector1*1);

            var lineSegmentList = new List<LineSegment2D>()
            {
                shearWall1,
                shearWall2
            };

            return lineSegmentList;
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
