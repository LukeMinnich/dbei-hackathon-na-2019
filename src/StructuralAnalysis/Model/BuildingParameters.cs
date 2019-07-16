using Kataclysm.Common;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class BuildingParameters
    {
        public ModelSettings ModelInformation { get; set; }
        public BuildingLevel SeismicBaseLevel { get; set; }
        public double ImportanceFactor { get; set; }
    }
}