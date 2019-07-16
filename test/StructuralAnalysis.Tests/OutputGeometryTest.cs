using System.Collections.Generic;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Kataclysm.StructuralAnalysis.Tests
{
    [TestFixture]
    public class OutputGeometryTest
    {
        [Test]
        public void GenerateGeometryForOutputJson()
        {
            var geometry = new Geometry()
            {
                BoundaryVertices = new List<Point2D>
                {
                    new Point2D(0, 0),
                    new Point2D(0, 240),
                    new Point2D(360, 240),
                    new Point2D(360, 0)
                },
                Wall = new List<WallGeometry>()
                {
                    new WallGeometry
                    {
                        EndI = new Point2D(0, 0),
                        EndJ = new Point2D(0, 240)
                    },
                    new WallGeometry
                    {
                        EndI = new Point2D(360, 0),
                        EndJ = new Point2D(360, 240)
                    },
                    new WallGeometry
                    {
                        EndI = new Point2D(0, 0),
                        EndJ = new Point2D(60, 0)
                    },
                    new WallGeometry
                    {
                        EndI = new Point2D(300, 0),
                        EndJ =  new Point2D(360, 0)
                    },
                    new WallGeometry
                    {
                        EndI = new Point2D(0, 240),
                        EndJ =  new Point2D(360, 240)
                    },
                }
            };

            var serialized = JsonConvert.SerializeObject(geometry, Formatting.Indented);
        }
    }
}