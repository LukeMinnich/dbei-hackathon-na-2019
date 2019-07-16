using Kataclysm.Common;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class NodalResponse : SuperimposableResponse<NodalResponse>
    {
        public Point2D Coordinate { get; set; }

        public NodalForce ExternalForce { get; set; } = new NodalForce();
        public NodalStress Stress { get; set; } = new NodalStress();
        public NodalForce Reaction { get; set; } = new NodalForce();
        public NodalDisplacement Displacement { get; set; } = new NodalDisplacement();  // TODO make sure these synchronize 

        public NodalResponse() {}

        [JsonConstructor]
        public NodalResponse(string elementId, LoadPattern loadPattern, Point2D coordinate, LoadCase loadCase = null)
            : base(elementId, loadPattern, loadCase)
        {
            Coordinate = coordinate;
        }

        public override NodalResponse ApplyLoadFactor(double loadFactor)
        {
            return new NodalResponse(ElementId, LoadPattern, Coordinate, LoadCase)
            {
                ExternalForce = new NodalForce
                {
                    LoadPattern = LoadPattern,
                    Fx = loadFactor * ExternalForce.Fx,
                    Fy = loadFactor * ExternalForce.Fy,
                    Mz = loadFactor * ExternalForce.Mz
                },
                Reaction = new NodalForce
                {
                    LoadPattern = LoadPattern,
                    Fx = loadFactor * Reaction.Fx,
                    Fy = loadFactor * Reaction.Fy,
                    Mz = loadFactor * Reaction.Mz
                },
                Displacement = new NodalDisplacement
                {
                    LoadPattern = LoadPattern,
                    Ux = loadFactor * Displacement.Ux,
                    Uy = loadFactor * Displacement.Uy,
                    Rz = loadFactor * Displacement.Rz
                },
                Stress = new NodalStress
                {
                    LoadPattern = LoadPattern,
                    Sigma_x = loadFactor * Stress.Sigma_x,
                    Sigma_xy = loadFactor * Stress.Sigma_xy,
                    Sigma_y = loadFactor * Stress.Sigma_y
                }
            };
        }

        public override NodalResponse Superimpose(NodalResponse other)
        {
            return new NodalResponse(ElementId, LoadPattern, Coordinate, LoadCase)
            {
                ExternalForce = new NodalForce
                {
                    LoadPattern = LoadPattern,
                    Fx = other.ExternalForce.Fx + ExternalForce.Fx,
                    Fy = other.ExternalForce.Fy + ExternalForce.Fy,
                    Mz = other.ExternalForce.Mz + ExternalForce.Mz
                },
                Reaction = new NodalForce
                {
                    LoadPattern = LoadPattern,
                    Fx = other.Reaction.Fx + Reaction.Fx,
                    Fy = other.Reaction.Fy + Reaction.Fy,
                    Mz = other.Reaction.Mz + Reaction.Mz
                },
                Displacement = new NodalDisplacement
                {
                    LoadPattern = LoadPattern,
                    Ux = other.Displacement.Ux + Displacement.Ux,
                    Uy = other.Displacement.Uy + Displacement.Uy,
                    Rz = other.Displacement.Rz + Displacement.Rz
                },
                Stress = new NodalStress
                {
                    LoadPattern = LoadPattern,
                    Sigma_x = other.Stress.Sigma_x + Stress.Sigma_x,
                    Sigma_xy = other.Stress.Sigma_xy + Stress.Sigma_xy,
                    Sigma_y = other.Stress.Sigma_y + Stress.Sigma_y
                }
            };
        }

        public static string FormatNodeId(int id)
        {
            return $"N{id:00000}";
        }

        public static string FormatNodeIdAlt(int id)
        {
            return $"RN{id:00000}";
        }
    }
}