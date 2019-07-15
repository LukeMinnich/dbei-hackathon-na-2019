using Kataclysm.Common.Units.Conversion;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class ShearWallPanelRigidAnalysisParameters
    {
        public ForcePerLength K { get; set; }
        public Unitless AngleFromWallAxisToGlobalXAxis { get; set; }
        
        /// <summary>
        /// A positive signed offset will yield positive Shear demand along the wall axis when applied story Torsion is positive
        /// </summary>
        public Length SignedOffsetFromCenterOfRigidity { get; set; }

        public ForcePerLength Kx => (ForcePerLength) (K * Result.Abs(Result.Cos(AngleFromWallAxisToGlobalXAxis)));
        public ForcePerLength Ky => (ForcePerLength) (K * Result.Abs(Result.Sin(AngleFromWallAxisToGlobalXAxis)));
    }
}