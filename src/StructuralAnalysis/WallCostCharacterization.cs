using Kataclysm.Randomizer;

namespace Kataclysm.StructuralAnalysis
{
    public class WallCostCharacterization
    {
        public RandomizedBuilding RandomizedBuilding { get; set; }
        public double GrossSquareFeet { get; set; }
        public double TotalStructuralShearWallCost { get; set; }
        public bool MeetsDriftLimit { get; set; }
        public Geometry Geometry { get; set; }
    }
}