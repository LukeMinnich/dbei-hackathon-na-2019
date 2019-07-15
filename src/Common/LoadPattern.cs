using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kataclysm.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LoadPattern
    {
        [Description("Dead")]
        Dead,
        [Description("Superimposed Dead")]
        SuperDead,
        [Description("Live Down")]
        Live_Down,
        [Description("Live Up")]
        Live_Up,
        [Description("Live Unreducible Down")]
        LiveUnreducible_Down,
        [Description("Live Unreducible Up")]
        LiveUnreducible_Up,
        [Description("Heavy Live Down")]
        LiveHeavy_Down,
        [Description("Heavy Live Up")]
        LiveHeavy_Up,
        [Description("Live Storage Down")]
        LiveStorage_Down,
        [Description("Live Storage Up")]
        LiveStorage_Up,
        [Description("Live Roof Down")]
        LiveRoof_Down,
        [Description("Live Roof Up")]
        LiveRoof_Up,
        [Description("Live Construction Down")]
        LiveConstruction_Down,
        [Description("Live Construction Up")]
        LiveConstruction_Up,
        [Description("Live Parking Down")]
        LiveParking_Down,
        [Description("Live Parking Up")]
        LiveParking_Up,
        [Description("Live Partition Down")]
        LivePartition_Down,
        [Description("Live Partition Up")]
        LivePartition_Up,
        [Description("Minimum Snow")]
        SnowMinimum,
        [Description("Balanced Snow")]
        SnowBalanced,
        [Description("Unbalanced Snow North")]
        SnowUnbalanced_North,
        [Description("Unbalanced Snow South")]
        SnowUnbalanced_South,
        [Description("Unbalanced Snow East")]
        SnowUnbalanced_East,
        [Description("Unbalanced Snow West")]
        SnowUnbalanced_West,
        [Description("Snow Drift North")]
        SnowDrift_North,
        [Description("Snow Drift South")]
        SnowDrift_South,
        [Description("Snow Drift East")]
        SnowDrift_East,
        [Description("Snow Drift West")]
        SnowDrift_West,
        [Description("Wind North")]
        Wind_North,
        [Description("Wind South")]
        Wind_South,
        [Description("Wind East")]
        Wind_East,
        [Description("Wind West")]
        Wind_West,
        [Description("Seismic North")]
        Seismic_North,
        [Description("Seismic South")]
        Seismic_South,
        [Description("Seismic East")]
        Seismic_East,
        [Description("Seismic West")]
        Seismic_West,
        [Description("Seismic Diaphragm North")]
        SeismicDiaphragm_North,
        [Description("Seismic Diaphragm South")]
        SeismicDiaphragm_South,
        [Description("Seismic Diaphragm East")]
        SeismicDiaphragm_East,
        [Description("Seismic Diaphragm West")]
        SeismicDiaphragm_West,
        [Description("Seismic Torsion X")]
        SeismicTorsion_XLoading,
        [Description("Seismic Torsion Y")]
        SeismicTorsion_YLoading,
        [Description("Wind Torsion East")]
        WindTorsion_East,
        [Description("Wind Torsion West")]
        WindTorsion_West,
        [Description("Wind Torsion North")]
        WindTorsion_North,
        [Description("Wind Torsion South")]
        WindTorsion_South,
        [Description("Unit Live")]
        UnitLiveLoad,
        [Description("Mixed")]
        Mixed
    }
}