using BuildingLayout;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class SeismicParameters
    {
        public SystemParameters SystemParameters { get; set; }
        public BuildingParameters BuildingParameters { get; set; }
        public Seismicity Seismicity { get; set; }

        public SeismicParameters()
        {

        }
        public SeismicParameters(SystemParameters systemParameters, BuildingParameters buildingParameters, Seismicity seismicity)
        {
            SystemParameters = systemParameters;
            BuildingParameters = buildingParameters;
            Seismicity = seismicity;
        }
    }
}