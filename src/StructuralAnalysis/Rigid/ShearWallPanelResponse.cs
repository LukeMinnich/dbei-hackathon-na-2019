using Kataclysm.Common;
using Kataclysm.Common.Units.Conversion;
using Katerra.Apollo.Structures.Common.Units;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class ShearWallPanelResponse : SuperimposableResponse<ShearWallPanelResponse>
    {
        public LineSegment2D WallLocation { get; set; }
        
        public Force DirectShear { get; set; } = new Force(0, ForceUnit.Kip);
        public Force TorsionalShear { get; set; } = new Force(0, ForceUnit.Kip);

        public Force TotalShear => (Force) (DirectShear + TorsionalShear);

        public ShearWallPanelResponse() {}

        [JsonConstructor]
        public ShearWallPanelResponse(string elementId, LoadPattern loadPattern, LineSegment2D wallLocation, LoadCase loadCase = null)
            : base(elementId, loadPattern, loadCase)
        {
            WallLocation = wallLocation;
        }
        
        public override ShearWallPanelResponse ApplyLoadFactor(double loadFactor)
        {
            return new ShearWallPanelResponse(ElementId, LoadPattern, WallLocation, LoadCase)
            {
                DirectShear = (Force) (loadFactor * DirectShear),
                TorsionalShear = (Force) (loadFactor * TorsionalShear)
            };
        }

        public override ShearWallPanelResponse Superimpose(ShearWallPanelResponse other)
        {
            return new ShearWallPanelResponse(ElementId, LoadPattern, WallLocation, LoadCase)
            {
                DirectShear = (Force) (other.DirectShear + DirectShear),
                TorsionalShear = (Force) (other.TorsionalShear + TorsionalShear)
            };
        }
    }
}