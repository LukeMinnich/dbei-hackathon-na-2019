using System;
using System.Collections.Generic;
using System.Linq;
using Kataclysm.Common;
using Kataclysm.Common.Units.Conversion;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class PointDrift : IReportsLevel
    {
        public string Name { get; set; }
        public BuildingLevel Level { get; set; }
        public LoadPatternType LoadCategory { get; set; }
        public LoadCase LoadCase { get; set; }
        public LateralDirection Direction { get; set; }
        public Point2D Coordinate { get; set; }
        public Point2D CoordinateBelow { get; set; }
        public Length Displacement { get; set; }
        public Length DisplacementBelow { get; set; }
        public Unitless Drift { get; set; }
        
        /// <summary>
        /// Gets matching drift for an unsorted collection of Point Drifts, matching according to Load Case and Direction
        /// </summary>
        /// <param name="driftsAtLevelBelow"></param>
        /// <returns></returns>
        public PointDrift GetMatchingDriftAtLevelBelow(IEnumerable<PointDrift> driftsAtLevelBelow)
        {
            List<PointDrift> driftsToCompare =
                driftsAtLevelBelow.Where(d => d.LoadCase == LoadCase && d.Direction == Direction).ToList();
            
            PointDrift closest = null;
            var closestDistance = Double.PositiveInfinity;

            for (var i = 0; i < driftsToCompare.Count; i++)
            {
                var other = driftsToCompare[i];
                var distance = Coordinate.DistanceTo(other.Coordinate);

                if (distance < closestDistance)
                {
                    closest = other;
                    closestDistance = distance;
                }
            }

            return closest;
        }
    }
}