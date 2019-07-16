using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using Kataclysm.Common.Geometry;
using Kataclysm.Common.Units.Conversion;
using Kataclysm.StructuralAnalysis.Model;
using Kataclysm.StructuralAnalysis.Rigid;
using Katerra.Apollo.Structures.Common.Units;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.StructuralAnalysis
{
    public class AnalysisManager
    {
        private SerializedModel _serializedModel;
        
        private List<LoadCase> _loadCases;
        private List<AnalyticalWallLateral> _lateralWallList;
        
        private List<BuildingLevelLateral2> _lateralLevels;
        
        private SeismicBuildingProperties _seismicBuildingProperties;
        
        private EquivalentLateralForceProcedure _elf;
        public LevelDataDictionary<RigidAnalysis> RigidAnalyses { get; private set; }

        public AnalysisManager(SerializedModel serializedModel)
        {
            _serializedModel = serializedModel;
            
            _loadCases = CommonResources.ASCE7LoadCases.Values.ToList();
            _lateralWallList = GetWallsFromBearingWalls(serializedModel.BearingWalls);
            
            _lateralLevels = BuildLateralLevels(_serializedModel.OneWayDecks);

            _seismicBuildingProperties = new SeismicBuildingProperties(_serializedModel.SeismicParameters,
                _serializedModel.ModelSettings, _lateralLevels);
        }

        private List<AnalyticalWallLateral> GetWallsFromBearingWalls(List<BearingWall> bearingWalls)
        {
            var shearWalls = bearingWalls.Where(w => w.IsShearWall && !w.HasOpening);

            var lateralWalls = new List<AnalyticalWallLateral>();
            
            foreach (BearingWall wall in shearWalls)
            {
                lateralWalls.Add(new AnalyticalWallLateral(wall));
            }

            return lateralWalls;
        }

        public WallCostCharacterization Run()
        {
            _elf = new EquivalentLateralForceProcedure(_lateralLevels, _seismicBuildingProperties,
                new Length(_serializedModel.ModelSettings.BuildingHeight, LengthUnit.Inch));
            
            _elf.Run();

            RigidAnalyses = GenerateRigidAnalyses();

            AnalyzeRigid();

            return CharacterizeAllWallCosts();
        }

        private WallCostCharacterization CharacterizeAllWallCosts()
        {
            
            
            foreach (BuildingLevelLateral2 level in _lateralLevels)
            {
                var analysis = RigidAnalyses[level.Level];
                
                
            }
            
            throw new NotImplementedException();
        }

        private List<BuildingLevelLateral2> BuildLateralLevels(List<OneWayDeck> decks)
        {
            var levels = new List<BuildingLevelLateral2>();

            foreach (OneWayDeck deck in decks)
            {
                levels.Add(new BuildingLevelLateral2(deck));
            }

            return levels;
        }

        private LevelDataDictionary<RigidAnalysis> GenerateRigidAnalyses()
        {
            var analyses = new LevelDataDictionary<RigidAnalysis>();

            foreach (BuildingLevelLateral2 level in _lateralLevels)
            {
                IEnumerable<AnalyticalWallLateral> wallsAtLevel = _lateralWallList.Where(w => w.TopLevel.Equals(level.Level));
                
                List<LateralLevelForce> forcesAtLevel = _elf.AppliedForces[level.Level];

                analyses.Add(new RigidAnalysis(level, wallsAtLevel, forcesAtLevel, CommonResources.ASCE7SeismicELFLoadCases(),
                    _serializedModel.SeismicParameters.SystemParameters.Cd));
            }

            return analyses;
        }

        private void AnalyzeRigid()
        {
            RigidAnalyses.Values.ForEach(a => a.Analyze());
        }
    }
}