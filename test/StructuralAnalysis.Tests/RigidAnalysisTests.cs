using System.Collections.Generic;
using System.Text;
using Kataclysm.Common;
using Kataclysm.Common.Units.Conversion;
using Kataclysm.StructuralAnalysis.Model;
using Kataclysm.StructuralAnalysis.Rigid;
using Katerra.Apollo.Structures.Common.Units;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace Kataclysm.StructuralAnalysis.Tests
{
    [TestFixture]
    public class RigidAnalysisTests
    {
        [Test]
        public void Analyze_Breyer6thEd_16_11_Succeeds()
        {
            var sB = new StringBuilder();
            
            var level = new BuildingLevel("Level 2", 100);

            var lateralWalls = new List<AnalyticalWallLateral>();

            lateralWalls.Add(new AnalyticalWallLateral("Wall A", new Point2D(0, 0), new Point2D(0, 240), level));

            lateralWalls.Add(new AnalyticalWallLateral("Wall B", new Point2D(360, 0), new Point2D(360, 240), level));

            lateralWalls.Add(new AnalyticalWallLateral("Wall C-1", new Point2D(0, 0), new Point2D(60, 0), level));
            lateralWalls.Add(new AnalyticalWallLateral("Wall C-2", new Point2D(300, 0), new Point2D(360, 0), level));

            lateralWalls.Add(new AnalyticalWallLateral("Wall D", new Point2D(0, 240), new Point2D(360, 240), level));

            var deck = new OneWayDeck
            {
                Level = level,
                Boundary = new Polygon2D(new List<Point2D>
                {
                    new Point2D(0, 0),
                    new Point2D(0, 240),
                    new Point2D(360, 240),
                    new Point2D(360, 0)
                }),
                WeightPerArea = new Stress(40, StressUnit.psf)
            };
            
            var lateralLevel = new BuildingLevelLateral2(deck);
            
            var forces = new List<LateralLevelForce>
            {
                new LateralLevelForce {DirectX = new Force(-6000, ForceUnit.Pound), LoadPattern = LoadPattern.Seismic_West, CenterOfForce = new Point2D(180, 120)},
                new LateralLevelForce {AccidentalT = new Moment(6, MomentUnit.KipFoot), LoadPattern = LoadPattern.SeismicTorsion_XLoading, CenterOfForce = new Point2D(180, 120)}
            };
            
            var loadCases = new List<LoadCase>
            {
                CommonResources.ASCE7LoadCases["-X - TX"],
            };
            
            var analysis = new RigidAnalysis(lateralLevel, lateralWalls, forces, loadCases, 1);
            
            analysis.Analyze();

            var table = RigidAnalysisTabularReport.GenerateShearWallForceStiffnessTable(analysis);

            sB.Append(table.PrintToMarkdown());
        }
    }
}