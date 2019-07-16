using System;
using System.Collections.Generic;
using Kataclysm.Common.Extensions;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.Common
{
    public abstract class LineLoad : FloorLoad
    {
        public LineSegment2D Line { get; set; }

        public double MagnitudeStart { get; set; }

        public double MagnitudeEnd { get; set; }

        public LineLoad(LineSegment2D line, double startMagnitude, double endMagnitude, BuildingLevel level, Projection projection, LoadPattern loadPattern):base(level, projection,loadPattern)
        {
            Line = line;
            MagnitudeStart = startMagnitude;
            MagnitudeEnd = endMagnitude;
        }

        public LineLoad(LineSegment2D line, double startMagnitude, double endMagnitude) : base()
        {
            Line = line;
            MagnitudeStart = startMagnitude;
            MagnitudeEnd = endMagnitude;
        }

        public LineLoad()
        {
        }
    }
}