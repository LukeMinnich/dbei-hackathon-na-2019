using System.ComponentModel;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public enum TorsionalIrregularityStatus
    {
        [Description("OK")]
        None,
        [Description("Type 1a")]
        Type1A,
        [Description("Type 1b")]
        Type1B
    }
}