using System.Collections.Generic;
using BuildingLayout;
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
    public class AnalysisManagerTests
    {
        [Test]
        public void SingleStory_E2E_Succeeds()
        {
            var level = new BuildingLevel("Level 02", 100);

            var serializedModel = new SerializedModel
            {
                BearingWalls = new List<BearingWall>
                {
                    new BearingWall
                    {
                        TopLevel = level,
                        EndI = new Point3D(0, 0, 100),
                        EndJ = new Point3D(0, 240, 100),
                        UniqueId = "Wall A",
                        HasOpening = false,
                        IsShearWall = true
                    },
                    new BearingWall
                    {
                        TopLevel = level,
                        EndI = new Point3D(360, 0, 100),
                        EndJ = new Point3D(360, 240, 100),
                        UniqueId = "Wall B",
                        HasOpening = false,
                        IsShearWall = true
                    },
                    new BearingWall
                    {
                        TopLevel = level,
                        EndI = new Point3D(0, 0, 100),
                        EndJ = new Point3D(60, 0, 100),
                        UniqueId = "Wall C-1",
                        HasOpening = false,
                        IsShearWall = true
                    },
                    new BearingWall
                    {
                        TopLevel = level,
                        EndI = new Point3D(300, 0, 100),
                        EndJ = new Point3D(360, 0, 100),
                        UniqueId = "Wall C-2",
                        HasOpening = false,
                        IsShearWall = true
                    },
                    new BearingWall
                    {
                        TopLevel = level,
                        EndI = new Point3D(0, 240, 100),
                        EndJ = new Point3D(360, 240, 100),
                        UniqueId = "Wall D",
                        HasOpening = false,
                        IsShearWall = true
                    }
                },
                OneWayDecks = new List<OneWayDeck>
                {
                    new OneWayDeck()
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
                    }
                },
                ModelSettings = new ModelSettings
                {
                    BuildingHeight = level.Elevation

                },
                SeismicParameters = new SeismicParameters
                {
                    BuildingParameters = new BuildingParameters()
                    {
                        ImportanceFactor = 1.0,
                        SeismicBaseLevel = new BuildingLevel("Level 01", 0)
                    },
                    Seismicity = Seismicity.High,
                    SystemParameters = new SystemParameters
                    {
                        Cd = 4,
                        R = 6.5,
                        Omega = 3,
                        Ct = 0.02,
                        X = 0.75
                    }
                }
            };
            
            var manager = new AnalysisManager(serializedModel);
            
            manager.Run();

            var table = RigidAnalysisTabularReport.GenerateShearWallForceStiffnessTable(manager.RigidAnalyses[level]);

            var output = table.PrintToMarkdown();
        }
    }
}