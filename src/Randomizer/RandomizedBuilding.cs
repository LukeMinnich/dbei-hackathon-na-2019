using BuildingLayout;

namespace Kataclysm.Randomizer
{
    public class RandomizedBuilding
    {
        public UnitMix UnitMix { get; set; }
        public Typology Typology { get; set; }
        public Seismicity Seismicity { get; set; }
        public int NumberOfStories { get; set; }
        public double DriftLimit { get; set; }
    }
}