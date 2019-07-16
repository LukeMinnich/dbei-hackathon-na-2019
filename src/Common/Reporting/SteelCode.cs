using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kataclysm.Common.Reporting
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SteelCode
    {
        [Description("AISC 360-10")]
        AISC_360_10,
        [Description("AISC 360-16")]
        AISC_360_16
    }
}