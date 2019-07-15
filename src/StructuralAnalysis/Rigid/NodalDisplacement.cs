using Kataclysm.Common;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class NodalDisplacement
    {
        public LoadPattern LoadPattern { get; set; }
        public double Ux { get; set; }
        public double Uy { get; set; }
        public double Rz { get; set; }

        public NodalDisplacement()
        {
        }
    }
}
