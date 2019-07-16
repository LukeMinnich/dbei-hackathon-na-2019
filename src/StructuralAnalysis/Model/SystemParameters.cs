namespace Kataclysm.StructuralAnalysis.Model
{
    public class SystemParameters
    {
        public double R { get; set; }
        public double Omega { get; set; }
        public double Cd { get; set; }
        public double Ct { get; set; }
        public double X { get; set; }

        public SystemParameters()
        {
        }

        public SystemParameters(double r, double omega, double cd, double ct, double x)
        {
            R = r;
            Omega = omega;
            Cd = cd;
            Ct = ct;
            X = x;
        }
    }
}