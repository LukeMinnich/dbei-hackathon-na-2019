using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingLayout
{
    public class UnitsLine
    {
        public List<BuildingUnit> BuildingUnits { get; set; }
        public Line2D Line { get; set; }

        public UnitsLine(Line2D line)
        {

        }
    }
}
