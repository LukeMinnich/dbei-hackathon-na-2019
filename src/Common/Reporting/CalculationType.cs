using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kataclysm.Common.Reporting
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CalculationType
    {
        FlexuralDesign,
        ShearDesign,
        CompressionDesign,
        TensionDesignYielding,
        TensionDesignRupture,
        AxialCompressionAndBendingInteraction,
        AxialTensionAndBendingInteraction,
        BendingAndShearInteraction,
        BendingAndWebCripplingInteraction,
        SeismicDesign,
        ShearWallShearPanelDesign,
        CommonProperties,
        DeflectionCheck,
        DiaphragmShearPanelDesign,
        StabilityDesign
    }
}
