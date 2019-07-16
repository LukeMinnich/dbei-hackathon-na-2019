using Kataclysm.Common;
using Kataclysm.Common.Units.Conversion;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class AccidentalEccentricities : IReportsLevel
    {
        public BuildingLevel Level { get; }
        
        public Unitless SeismicX { get; set; }
        public Unitless SeismicY { get; set; }
        public Unitless AmplifiedSeismicX { get; set; }
        public Unitless AmplifiedSeismicY { get; set; }
        public Unitless WindX { get; set; }
        public Unitless WindY { get; set; }

        [JsonConstructor]
        private AccidentalEccentricities() {}

        public static AccidentalEccentricities DefaultASCE7
        {
            get
            {
                return new AccidentalEccentricities
                {
                    SeismicX = new Unitless(0.05),
                    SeismicY = new Unitless(0.05),
                    AmplifiedSeismicX = new Unitless(0.05),
                    AmplifiedSeismicY = new Unitless(0.05),
                    WindX = new Unitless(0.15),
                    WindY = new Unitless(0.15)
                };
            }
        }
    }
}