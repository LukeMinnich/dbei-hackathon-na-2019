using Kataclysm.Common;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class BuildingParameters
    {
        public BuildingLevel SeismicBaseLevel { get; set; }
        public double ImportanceFactor { get; set; } = 1.0;

        public BuildingParameters()
        {

        }
        public BuildingParameters(BuildingLevel seismicBaseLevel)
        {
            SeismicBaseLevel = seismicBaseLevel;
        }
    }
}