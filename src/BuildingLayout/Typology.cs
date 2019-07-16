using Kataclysm.Common;

namespace BuildingLayout
{
    public class Typology : Enumeration
    {
        public static readonly Typology M = new Typology(0, "M");
        public static readonly Typology T = new Typology(0, "T");
        public static readonly Typology L = new Typology(0, "L");
        public static readonly Typology I = new Typology(0, "I");
        public static readonly Typology U = new Typology(0, "U");
        
        public Typology(int value, string description)
            : base(value, description)
        {
        }
    }
}