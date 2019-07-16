using System.Collections.Generic;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.StructuralAnalysis
{
    public class Geometry
    {
        public List<Point2D> BoundaryVertices { get; set; } = new List<Point2D>();
        public List<WallGeometry> Wall { get; set; } = new List<WallGeometry>();
    }
}