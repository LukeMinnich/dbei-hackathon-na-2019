using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kataclysm.Common.Reporting
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GoverningCode
    {
        [Description("ASCE 7-10")]
        ASCE_7_10,
        [Description("ASCE 7-16")]
        ASCE_7_16,
        [Description("IBC 2012")]
        ICC_IBC_2012,
        [Description("IBC 2015")]
        ICC_IBC_2015
    }
}