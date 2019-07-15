using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kataclysm.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Projection
    {
        Vertical,
        AlongSlope,
        NormalToSlope
    }
}