using System.ComponentModel;

namespace Kataclysm.Common
{
    public enum LoadPatternType
    {
        [Description("Self Dead")]
        SelfDead,
        [Description("Superimposed Dead")]
        SuperimposedDead,
        [Description("Live")]
        Live,
        [Description("Live Roof")]
        LiveRoof,
        [Description("Live Storage")]
        LiveStorage,
        [Description("Snow")]
        Snow,
        [Description("Wind")]
        Wind,
        [Description("Seismic")]
        Earthquake,
        [Description("Seismic Diaphragm")]
        Earthquake_Diaphragm,
        [Description("Unit Live")]
        UnitLive,
        [Description("Mixed")]
        Mixed
    }
}
