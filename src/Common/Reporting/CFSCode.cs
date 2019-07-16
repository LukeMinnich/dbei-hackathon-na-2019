using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kataclysm.Common.Reporting
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CFSCode
    {
        [Description("AISI S100-16")]
        AISI_S100_16,
        [Description("AISI S400-15")]
        AISI_S400_15,
        [Description("AISI S240-15")]
        AISI_S240_15
    }
}