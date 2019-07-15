using System;

namespace Kataclysm.Common
{
    public static class LoadPatternTypeConverter
    {
        public static LoadPatternType Convert(LoadPattern loadPattern)
        {
            switch (loadPattern)
            {
                case LoadPattern.Dead:
                    return LoadPatternType.SelfDead;
                case LoadPattern.SuperDead:
                    return LoadPatternType.SuperimposedDead;
                case LoadPattern.Live_Down:
                case LoadPattern.Live_Up:
                case LoadPattern.LiveUnreducible_Down:
                case LoadPattern.LiveUnreducible_Up:
                case LoadPattern.LiveHeavy_Down:
                case LoadPattern.LiveHeavy_Up:
                    return LoadPatternType.Live;
                case LoadPattern.LiveStorage_Down:
                case LoadPattern.LiveStorage_Up:
                    return LoadPatternType.LiveStorage;
                case LoadPattern.LiveRoof_Down:
                case LoadPattern.LiveRoof_Up:
                    return LoadPatternType.LiveRoof;
                case LoadPattern.LiveConstruction_Down:
                case LoadPattern.LiveConstruction_Up:
                case LoadPattern.LiveParking_Down:
                case LoadPattern.LiveParking_Up:
                case LoadPattern.LivePartition_Down:
                case LoadPattern.LivePartition_Up:
                    return LoadPatternType.Live;
                case LoadPattern.SnowMinimum:
                case LoadPattern.SnowBalanced:
                case LoadPattern.SnowUnbalanced_North:
                case LoadPattern.SnowUnbalanced_South:
                case LoadPattern.SnowUnbalanced_East:
                case LoadPattern.SnowUnbalanced_West:
                case LoadPattern.SnowDrift_North:
                case LoadPattern.SnowDrift_South:
                case LoadPattern.SnowDrift_East:
                case LoadPattern.SnowDrift_West:
                    return LoadPatternType.Snow;
                case LoadPattern.Wind_North:
                case LoadPattern.Wind_South:
                case LoadPattern.Wind_East:
                case LoadPattern.Wind_West:
                case LoadPattern.WindTorsion_North:
                case LoadPattern.WindTorsion_South:
                case LoadPattern.WindTorsion_East:
                case LoadPattern.WindTorsion_West:
                    return LoadPatternType.Wind;
                case LoadPattern.Seismic_North:
                case LoadPattern.Seismic_South:
                case LoadPattern.Seismic_East:
                case LoadPattern.Seismic_West:
                case LoadPattern.SeismicTorsion_XLoading:
                case LoadPattern.SeismicTorsion_YLoading:
                    return LoadPatternType.Earthquake;
                case LoadPattern.SeismicDiaphragm_North:
                case LoadPattern.SeismicDiaphragm_South:
                case LoadPattern.SeismicDiaphragm_East:
                case LoadPattern.SeismicDiaphragm_West:
                    return LoadPatternType.Earthquake_Diaphragm;
                case LoadPattern.UnitLiveLoad:
                    return LoadPatternType.UnitLive;
                case LoadPattern.Mixed:
                    return LoadPatternType.Mixed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(loadPattern), loadPattern, null);
            }
        }
    }
}