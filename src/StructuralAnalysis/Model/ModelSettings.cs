namespace Kataclysm.StructuralAnalysis.Model
{
    public struct ModelSettings
    {
        public double BuildingHeight { get; set; }

        public ModelSettings(double buildingHeight)
        {
            BuildingHeight = buildingHeight;
        }
    }
}