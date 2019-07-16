using Kataclysm.Common;

namespace BuildingLayout
{
    public class UnitMix : Enumeration
    {
        public double S { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        
        public static readonly UnitMix Small = new UnitMix(0, "Small", 0.30, 0.40, 0.30, 0); 
        public static readonly UnitMix Medium = new UnitMix(1, "Medium", 0.15, 0.15, 0.40, 0.30);
        public static readonly UnitMix Large = new UnitMix(2, "Large", 0, 0.30, 0.30, 0.40); 
        
        public UnitMix(int value, string description, double s, double a, double b, double c)
            : base(value, description)
        {
            S = s;
            A = a;
            B = b;
            C = c;
        }
    }
}