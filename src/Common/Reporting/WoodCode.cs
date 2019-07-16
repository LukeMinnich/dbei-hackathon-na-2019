using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kataclysm.Common.Reporting
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WoodCode
    {
        [Description("NDS 2012")]
        AWC_NDS_2012,
        [Description("NDS 2015")]
        AWC_NDS_2015
    }
}