using Kataclysm.Common;

namespace BuildingLayout
{
    public class Seismicity : Enumeration
    {
        public SiteParameters SiteParameters { get; set; }
        
        // Charlestown, SC
        public static readonly Seismicity Moderate = new Seismicity(0, "Medium", new SiteParameters()
        {
            Ss = 1.097,
            S1 = 0.348,
            SMS = 1.164,
            SM1 = 0.593,
            SDS = 0.776,
            SD1 = 0.395,
            Fa = 1.061,
            Fv = 1.704,
            TL = 8
        });
        
        // San Francisco
        public static readonly Seismicity High = new Seismicity(1, "High", new SiteParameters()
        {
            Ss = 1.5,
            S1 = 0.638,
            SMS = 1.5,
            SM1 = 0.957,
            SDS = 1.0,
            SD1 = 0.638,
            Fa = 1.0,
            Fv = 1.5,
            TL = 12
        });
        
        // Los Angeles, CA
        public static readonly Seismicity VeryHigh = new Seismicity(1, "Very High", new SiteParameters()
        {
            Ss = 2.432,
            S1 = 0.853,
            SMS = 2.432,
            SM1 = 1.279,
            SDS = 1.622,
            SD1 = 0.853,
            Fa = 1.0,
            Fv = 1.5,
            TL = 8
        });
        
        public Seismicity(int value, string description, SiteParameters siteParameters)
            : base(value, description)
        {
            SiteParameters = siteParameters;
        }
    }
}