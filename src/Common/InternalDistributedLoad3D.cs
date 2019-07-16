using Newtonsoft.Json;

namespace Kataclysm.Common
{
    public class InternalDistributedLoad3D : DistributedLoad3D, IInternalLoad
    {
        [JsonIgnore]
        public AnalyticalElement Source { get; }
        
        public InternalDistributedLoad3D(PointLoad3D startLoad, PointLoad3D endLoad, AnalyticalElement source)
            : base(startLoad, endLoad)
        {
            Source = source;
        }
    }
}
