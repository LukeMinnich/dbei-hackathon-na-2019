using System;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.Common
{
    public class InternalPointLoad3D : PointLoad3D, IInternalLoad
    {
        [JsonIgnore]
        private AnalyticalElement _source;

        public InternalPointLoad3D(Point3D location, Vector3D forces, Vector3D moments, AnalyticalElement source,
            LoadPattern loadPattern) :
            base(location, forces, moments, loadPattern)
        {
            _source = source;
        }

        public InternalPointLoad3D(Vector3D forces, Vector3D moments, AnalyticalElement source) : base(forces, moments)
        {
            _source = source;
        }

        public AnalyticalElement Source
        {
            get { return _source; }
        }

        public InternalPointLoad3D Combine(InternalPointLoad3D other)
        {
            if (Location != other.Location)
                throw new InvalidOperationException(
                    $"Cannot combined loads at {Location} and {other.Location}. They are not coincident");

            return new InternalPointLoad3D(Location, Forces + other.Forces, Moments + other.Moments, _source,
                LoadPattern);
        }
    }
}