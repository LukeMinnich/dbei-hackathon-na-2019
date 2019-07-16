using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kataclysm.Common.Reporting
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResultLimitType
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Override,
        MaximumOf,
        MinimumOf
    }
}