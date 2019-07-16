using System.Collections.Generic;
using Kataclysm.Common;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class MassCenter
    {
        public Point2D CenterOfMass { get; set; }
        public double Weight { get; set; }
        public List<AreaLoad> DeadLoads { get; set; }

        public MassCenter(Point2D centroid, double weight)
        {
            CenterOfMass = centroid;
            Weight = weight;
        }
    }
}
