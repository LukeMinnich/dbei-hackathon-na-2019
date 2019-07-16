using Kataclysm.Common;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class NodalStress
    {
        public double Sigma_x { get; set; }
        public double Sigma_y { get; set; }
        public double Sigma_xy { get; set; }
        public LoadPattern LoadPattern { get; set; }
        
        public NodalStress() {}

        public NodalStress(double sigmaX, double sigmaY, double sigmaXy, LoadPattern loadPattern)
        {
            Sigma_x = sigmaX;
            Sigma_y = sigmaY;
            Sigma_xy = sigmaXy;
            LoadPattern = loadPattern;
        }
    }
}
