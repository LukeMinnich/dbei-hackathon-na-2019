using System.Collections.Generic;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class RigidResponses
    {
        public Dictionary<string, Point2D> TorsionalIrregularityTrackingNodes { get; set; }
        
        public Dictionary<string, ShearWallPanelResponseCollection> WallLoadPatternResults { get; set; }
        public Dictionary<string, ShearWallPanelResponseCollection> WallLoadCaseResults { get; set; }
        
        public Dictionary<string, NodalResponseCollection> NodalLoadPatternResults { get; set; }
        public Dictionary<string, NodalResponseCollection> NodalLoadCaseResults { get; set; }
    }
}