using System;
using System.Collections.Generic;
using System.Linq;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public abstract class GeneralLightFramedShearWall : GeneralLightFramedWall
    {
        #region Public Properties
        public TensionRestraintType RestraintType { get; set; }
        public List<LateralLoad> CumulativeLateralLoads { get; set; } = new List<LateralLoad>(); //List of cumulative lateral loads, including both wind and seismic
        public List<LateralLoad> IncrementalLateralLoads { get; set; } = new List<LateralLoad>(); //List of incremental lateral loads, including both wind and seismic
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the cumulative lateral load for the given load pattern and lateral load type.
        /// </summary>
        /// <param name="loadPattern">Lateral load pattern to query (Earthquake or Wind)</param>
        /// <param name="loadType">Load type to query (Lateral Shear, Lateral Unit Shear, Lateral Overturning Moment)</param>
        /// <returns></returns>
        public double GetCumulativeLateralLoad(LoadPatternType loadPattern, LoadType loadType)
        {
            var loadsMatchingPattern = CumulativeLateralLoads.Where(l => l.LoadPattern == loadPattern);
            
            double totalLoad = 0;

            foreach (LateralLoad load in loadsMatchingPattern)
            {
                if (loadType == LoadType.LateralShear)
                {
                    if (load.LoadType == LoadType.LateralShear)
                    {
                        totalLoad += load.Magnitude;
                    }
                    else if (load.LoadType == LoadType.LateralUnitShear)
                    {
                        totalLoad += load.Magnitude * WallLength;
                    }
                }
                else if (loadType == LoadType.LateralUnitShear)
                {
                    if (load.LoadType == LoadType.LateralShear)
                    {
                        totalLoad += load.Magnitude / WallLength;
                    }
                    else if (load.LoadType == LoadType.LateralUnitShear)
                    {
                        totalLoad += load.Magnitude;
                    }
                }
                else if (loadType == LoadType.LateralOverturningMoment)
                {
                    totalLoad += load.Magnitude;
                }
            }

            return totalLoad;
        }

        /// <summary>
        /// Returns the incremental lateral load for the given load pattern and lateral load type.
        /// </summary>
        /// <param name="loadPattern">Lateral load pattern to query (Earthquake or Wind)</param>
        /// <param name="loadType">Load type to query (Lateral Shear, Lateral Unit Shear, Lateral Overturning Moment)</param>
        /// <returns></returns>
        public double GetIncrementalLateralLoad(LoadPatternType loadPattern, LoadType loadType)
        {
            var loadsMatchingPattern = CumulativeLateralLoads.Where(l => l.LoadPattern == loadPattern);
            
            double totalLoad = 0;
            
            foreach (LateralLoad load in loadsMatchingPattern)
            {
                if (loadType == LoadType.LateralShear)
                {
                    if (load.LoadType == LoadType.LateralShear)
                    {
                        totalLoad += load.Magnitude;
                    }
                    else if (load.LoadType == LoadType.LateralUnitShear)
                    {
                        totalLoad += load.Magnitude * WallLength;
                    }
                }
                else if (loadType == LoadType.LateralUnitShear)
                {
                    if (load.LoadType == LoadType.LateralShear)
                    {
                        totalLoad += load.Magnitude / WallLength;
                    }
                    else if (load.LoadType == LoadType.LateralUnitShear)
                    {
                        totalLoad += load.Magnitude;
                    }
                }
                else if (loadType == LoadType.LateralOverturningMoment)
                {
                    totalLoad += load.Magnitude;
                }
            }

            return totalLoad;
        }
        
        #endregion

        #region Public Abstract Methods
        
        public abstract LightFramedWallFiniteElementModel BuildWallFEAModel(bool isBase, bool isTop, LightFramedWallFiniteElementModel wallBelow);
        
        #endregion

        #region Protected Methods
        
        /// <summary>
        /// Gets the cumulative factored overturning moment for the given load combination.
        /// </summary>
        /// <param name="LC">Load combination</param>
        /// <returns></returns>
        protected double GetFactoredOverturningMoment(LoadCombination LC)
        {
            return Math.Abs(CumulativeLateralLoads.Where(l => l.LoadType == LoadType.LateralOverturningMoment)
                                             .Sum(l => l.Magnitude * LC.GetLoadFactor(l.LoadPattern)));
        }

        /// <summary>
        /// Gets the incremental factored overturning moment for the given load combination.
        /// </summary>
        /// <param name="LC">Load combination</param>
        /// <returns></returns>
        protected double GetIncrementalFactoredOverturningMoment(LoadCombination LC)
        {
            return Math.Abs(IncrementalLateralLoads.Where(l => l.LoadType == LoadType.LateralOverturningMoment)
                                              .Sum(l => l.Magnitude * LC.GetLoadFactor(l.LoadPattern)));
        }

        /// <summary>
        /// Returns the maximum factored wind and seismic lateral shear demands, including their associated load combinations.  Lateral shears are returned as unit shears (kips/in).
        /// </summary>
        /// <returns></returns>
        protected MaxLateralShear GetMaximumLateralShears()
        {
            double MaxSeismicShear = 0;
            LoadCombination SeismicLoadCombo = null;
            double MaxWindShear = 0;
            LoadCombination WindLoadCombo = null;

            foreach (LoadCombination LC in LoadCombos)
            {
                double SeismicShear = 0;
                double WindShear = 0;
                foreach (LateralLoad Lat in CumulativeLateralLoads)
                {
                    if (Lat.LoadPattern.Equals(LoadPatternType.Earthquake) == true)
                    {
                        if (Lat.LoadType.Equals(LoadType.LateralUnitShear) == true)
                        {
                            //Load defined as unit shear
                            SeismicShear += Lat.Magnitude * LC.EQ;
                        }
                        else if (Lat.LoadType.Equals(LoadType.LateralShear) == true)
                        {
                            //Load defined as total shear force
                            SeismicShear += Lat.Magnitude * LC.EQ / WallLength;
                        }
                    }
                    else if (Lat.LoadPattern.Equals(LoadPatternType.Wind) == true)
                    {
                        if (Lat.LoadType.Equals(LoadType.LateralUnitShear) == true)
                        {
                            //Load defined as unit shear
                            WindShear += Lat.Magnitude * LC.W;

                        }
                        else if (Lat.LoadType.Equals(LoadType.LateralShear) == true)
                        {
                            //Load defined as total shear force
                            WindShear += Lat.Magnitude * LC.W / WallLength;
                        }
                    }
                }

                if (Math.Abs(SeismicShear) > MaxSeismicShear)
                {
                    MaxSeismicShear = Math.Abs(SeismicShear);
                    SeismicLoadCombo = LC;
                }

                if (Math.Abs(WindShear) > MaxWindShear)
                {
                    MaxWindShear = Math.Abs(WindShear);
                    WindLoadCombo = LC;
                }
            }

            return new MaxLateralShear(MaxSeismicShear, MaxWindShear, SeismicLoadCombo, WindLoadCombo);
        }
        
        #endregion
    }
}
