using System;

namespace Kataclysm.Common.Geometry
{
    public class MinMaxBounds
    {
        public double MaxX { get; set; }
        public double MinX { get; set; }
        public double MaxY { get; set; }
        public double MinY { get; set; }

        public MinMaxBounds GetMaximumBounds(MinMaxBounds other)
        {
            MinMaxBounds newBounds = new MinMaxBounds();

            newBounds.MaxX = Math.Max(MaxX, other.MaxX);
            newBounds.MaxY = Math.Max(MaxY, other.MaxY);
            newBounds.MinX = Math.Min(MinX, other.MinX);
            newBounds.MinY = Math.Min(MinY, other.MinY);

            return newBounds;
        }
    }
}
