using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kataclysm.Common;
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
        private Dictionary<BuildingLevel, MassCenter> _levelMasses;
        private SeismicBuildingProperties _seismicBuildingProperties;
        private List<AnalyticalWallLateral> _lateralWallList;
        
        private EquivalentLateralForceProcedure _elf;
        
        private List<BuildingLevelLateral2> _lateralLevels;
        private LevelDataDictionary<RigidAnalysis> _rigidAnalyses;
        private List<LoadCase> _loadCases;
        private LevelDictionary<PointDrift> _torsionalIrregularityDriftsAtBoundaryCorners;

        public AnalysisManager(SerializedModel serModel, SerializedModel serializedModel)
        {
            _serializedModel = serModel;
            _loadCases = CommonResources.ASCE7LoadCases.Values.ToList();
        }

        public void Run()
        {
            // Revised Rigid Analysis
            _lateralLevels = BuildLateralLevels(_levelMasses, _serializedModel.OneWayDecks);
            
            _elf = new EquivalentLateralForceProcedure(_lateralLevels, _seismicBuildingProperties,
                new Length(_serializedModel.ModelSettings.BuildingHeight, LengthUnit.Inch));
            
            _elf.Run();

            _rigidAnalyses = GenerateRigidAnalyses();

            AnalyzeRigid();
        }

        private List<BuildingLevelLateral2> BuildLateralLevels(Dictionary<BuildingLevel, MassCenter> levelMasses,
            List<OneWayDeck> decks)
        {
            var levels = new List<BuildingLevelLateral2>();

            foreach (BuildingLevel level in levelMasses.Keys)
            {
                List<OneWayDeck> decksAtLevel = decks.Where(d => d.Level.Equals(level)).ToList();
                
                Debug.Assert(decksAtLevel.Count== 1);

                Polygon2D boundary = decksAtLevel[0].Boundary.ProjectToHorizontalPlane();

                levels.Add(new BuildingLevelLateral2(level, levelMasses[level], boundary));
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

                analyses.Add(new RigidAnalysis(level, wallsAtLevel, forcesAtLevel, _loadCases,
                    _serializedModel.SeismicParameters.SystemParameters.Cd));
            }

            return analyses;
        }

        private void AnalyzeRigid()
        {
            _rigidAnalyses.Values.ForEach(a => a.Analyze());
        }
        
    }
}