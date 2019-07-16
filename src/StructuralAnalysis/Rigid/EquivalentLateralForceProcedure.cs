using System.Collections.Generic;
using System.Linq;
using Kataclysm.Common;
using Kataclysm.Common.Reporting;
using Kataclysm.Common.Units.Conversion;
using Katerra.Apollo.Structures.Common.Units;
using MathNet.Spatial.Euclidean;
using static Kataclysm.StructuralAnalysis.ASCE7_10_Equations;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class EquivalentLateralForceProcedure
    {
        public LevelDictionary<LateralLevelForce> AppliedForces { get; private set; }
        
        private List<BuildingLevelLateral2> _levels;
        private SeismicBuildingProperties _seismicProperties;
        private Length _buildingHeight;
        private Time _fundamentalPeriod;
        private Force _baseShear;

        private LevelDataDictionary<VerticalDistributionFactors> _distributionFactors;
        private LevelDataDictionary<SeismicStoryForce> _storyForces;
        private LevelDataDictionary<SeismicStoryForce> _storyShears; 
        
        public EquivalentLateralForceProcedure(IEnumerable<BuildingLevelLateral2> levels,
            SeismicBuildingProperties seismicProperties, Length buildingHeight)
        {
            _levels = levels.ToList();
            _seismicProperties = seismicProperties;
            _buildingHeight = buildingHeight;
        }

        public void Run()
        {
            ValidateLevelHeights();

            SetDefaultStoryEccentricities();

            _baseShear = DetermineSeismicBaseShear();

            _distributionFactors =  DetermineDistributionFactors();

            _storyForces = DetermineStoryForces();

            _storyShears = DetermineStoryShears();

            AppliedForces = GenerateAppliedLoading();
        }

        private LevelDictionary<LateralLevelForce> GenerateAppliedLoading()
        {
            var appliedLoading = new LevelDictionary<LateralLevelForce>();

            foreach (BuildingLevelLateral2 lateralLevel in _levels)
            {
                appliedLoading.Add(lateralLevel.Level, GenerateAppliedLoadingAtLevel(lateralLevel));
            }

            return appliedLoading;
        }

        private LevelDataDictionary<SeismicStoryForce> DetermineStoryShears()
        {
            var storyShears = new LevelDataDictionary<SeismicStoryForce>();
            
            var calc = new RecordedCalculation("Design Story Shears", CalculationType.SeismicDesign);

            List<BuildingLevelLateral2> levelsFromBottomToTop = _levels.OrderBy(l => l.Level.Elevation).ToList();

            for (var i = 0; i < levelsFromBottomToTop.Count; i++)
            {
                List<SeismicStoryForce> orderedForces = _storyForces.DataOrderedFromBottomToTopLevel();
                
                Force Vx_i_x = Chapter_12.Section_12_8.Eqn_12_8_13(orderedForces.Select(f => f.X).ToList(), i, ref calc);
                Force Vx_i_y = Chapter_12.Section_12_8.Eqn_12_8_13(orderedForces.Select(f => f.Y).ToList(), i, ref calc);
                
                storyShears.Add(new SeismicStoryForce
                {
                    Level = levelsFromBottomToTop[i].Level,
                    X = Vx_i_x,
                    Y = Vx_i_y
                });
            };

            return storyShears;
        }

        private LevelDataDictionary<SeismicStoryForce> DetermineStoryForces()
        {
            var storyForces = new LevelDataDictionary<SeismicStoryForce>();
            
            var calc = new RecordedCalculation("Design Story Forces", CalculationType.SeismicDesign);
            
            
            foreach (BuildingLevelLateral2 level in _levels)
            {
                var distributionFactorX = _distributionFactors[level.Level].X;
                var distributionFactorY = _distributionFactors[level.Level].Y;
                
                Force Fx_i_x = Chapter_12.Section_12_8.Eqn_12_8_11(distributionFactorX, _baseShear, ref calc);
                Force Fx_i_y = Chapter_12.Section_12_8.Eqn_12_8_11(distributionFactorY, _baseShear, ref calc);
                
                storyForces.Add(new SeismicStoryForce
                {
                    Level = level.Level,
                    X = Fx_i_x,
                    Y = Fx_i_y
                });
            }
            
//            CalcLog.AddCalculation(calc);

            return storyForces;
        }

        private LevelDataDictionary<VerticalDistributionFactors> DetermineDistributionFactors()
        {
            var distributionFactors = new LevelDataDictionary<VerticalDistributionFactors>();

            var k = 1.0;
            if (_fundamentalPeriod.Value > 0.5 && _fundamentalPeriod.Value < 2.5)
            {
                k = 1.0 + (_fundamentalPeriod.Value - 0.5) / 2d;
            }
            else if (_fundamentalPeriod.Value >= 2.5)
            {
                k = 2.0;
            }
            
            List<Force> w = _levels.Select(l => l.SeismicWeight).ToList();
            List<Length> H = _levels.Select(l => l.Height).ToList();

            var n = _levels.Count;
            
            var calc = new RecordedCalculation("Vertical Distribution Factors", CalculationType.SeismicDesign);

            foreach (BuildingLevelLateral2 level in _levels)
            {
                Force w_x = level.SeismicWeight;
                Length h_x = level.Height;

                Unitless C_vxi = Chapter_12.Section_12_8.Eqn_12_8_12(w_x, h_x, k, n, w, H, ref calc);
                                
                distributionFactors.Add(new VerticalDistributionFactors
                {
                    Level = level.Level,
                    X = C_vxi,
                    Y = C_vxi
                });
            }
                
//            CalcLog.AddCalculation(calc);

            return distributionFactors;
        }

        private Force DetermineSeismicBaseShear()
        {
            var baseShearCalc = new RecordedCalculation("Seismic Parameter Calculations", CalculationType.SeismicDesign);
                        
            var seismicParameters = _seismicProperties.SeismicParameters;
            
            double C_t = seismicParameters.SystemParameters.Ct;
            double X = seismicParameters.SystemParameters.X;
            double I_e = seismicParameters.BuildingParameters.ImportanceFactor;

            double S_1 = seismicParameters.Seismicity.SiteParameters.S1;
            double S_D1 = seismicParameters.Seismicity.SiteParameters.SD1;
            double S_DS = seismicParameters.Seismicity.SiteParameters.SDS;
            double R = seismicParameters.SystemParameters.R;
            
            Time T_L = new Time(seismicParameters.Seismicity.SiteParameters.TL);

            Force W = new Force(_seismicProperties.BuildingWeight, ForceUnit.Kip);

            Time T_a = Chapter_12.Section_12_8.Eqn_12_8_7(_buildingHeight, C_t, X, ref baseShearCalc);
            
            _fundamentalPeriod = T_a;
            
            Unitless C_s = Chapter_12.Section_12_8.CalculateCs(I_e, S_1, S_D1, S_DS, T_L, R,
                _fundamentalPeriod, ref baseShearCalc);
            
//            CalcLog.AddCalculation(baseShearCalc);
            
            return Chapter_12.Section_12_8.Eqn_12_8_1(C_s, W, ref baseShearCalc);
        }

        private void SetDefaultStoryEccentricities()
        {
            _levels.ForEach(l => l.Eccentricities = AccidentalEccentricities.DefaultASCE7);
        }

        private void ValidateLevelHeights()
        {
            if (_levels.Any(l => l.Height == null))
            {
                var resolver = new LevelHeightResolver(_levels,
                    _seismicProperties.SeismicParameters.BuildingParameters.SeismicBaseLevel);

                resolver.Resolve();
            }
        }

        // Assumes that the centers of mass stack vertically, so we can apply the design story shear directly
        public List<LateralLevelForce> GenerateAppliedLoadingAtLevel(BuildingLevelLateral2 lateralLevel)
        {
            var loads = new List<LateralLevelForce>();

            BuildingLevel level = lateralLevel.Level;
            Point2D CenterOfMass = lateralLevel.CenterOfMass;

            SeismicStoryForce storyShears = _storyShears[level];
            
            // Calculate direct seismic forces
            loads.Add(new LateralLevelForce
            {
                Level = level,
                DirectX = storyShears.X,
                LoadPattern = LoadPattern.Seismic_East,
                CenterOfForce = CenterOfMass
            });
            
            loads.Add(new LateralLevelForce
            {
                Level = level,
                DirectX = (Force) (-storyShears.X),
                LoadPattern = LoadPattern.Seismic_West,
                CenterOfForce = CenterOfMass
            });
            
            loads.Add(new LateralLevelForce
            {
                Level = level,
                DirectY = storyShears.Y,
                LoadPattern = LoadPattern.Seismic_North,
                CenterOfForce = CenterOfMass
            });
            
            loads.Add(new LateralLevelForce
            {
                Level = level,
                DirectY = (Force) (-storyShears.Y),
                LoadPattern = LoadPattern.Seismic_South,
                CenterOfForce = CenterOfMass
            });
            
            // Calculate accidental seismic forces
            Length lengthX = lateralLevel.LengthX;
            Length lengthY = lateralLevel.LengthY;
            AccidentalEccentricities eccentricities = lateralLevel.Eccentricities;
            
            loads.Add(new LateralLevelForce
            {
                Level = level,
                AccidentalT = (Moment) Result.Abs(eccentricities.SeismicX * lengthY * storyShears.X),
                LoadPattern = LoadPattern.SeismicTorsion_XLoading,
                CenterOfForce = CenterOfMass
            });
            
            loads.Add(new LateralLevelForce
            {
                Level = level,
                AccidentalT = (Moment) Result.Abs(eccentricities.SeismicY * lengthX * storyShears.Y),
                LoadPattern = LoadPattern.SeismicTorsion_YLoading,
                CenterOfForce = CenterOfMass
            });

            return loads;
        }
    }
}