namespace Kataclysm.StructuralAnalysis
{
    public class WallCostCharacterization
    {
        public string UnitMix { get; set; }
        public string Typology { get; set; }
        public string Seismicity { get; set; }
        public int NumberOfStories { get; set; }
        public double DriftLimit { get; set; }
        public double GrossSquareFeet { get; set; }
        public double TotalStructuralShearWallCost { get; set; }
        public bool MeetsDriftLimit { get; set; }
        public Geometry Geometry { get; set; }
    }
}