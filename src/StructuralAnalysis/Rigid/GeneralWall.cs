using Kataclysm.Common;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public abstract class GeneralWall
    {
        public BuildingLevel BottomLevel { get; set; }
        public BuildingLevel TopLevel { get; set; }
        public double WallHeight { get; set; } //Height of wall frome defined geometry
        public string WallID { get; set; } //Unique Wall ID
        public double WallThickness { get; set; } //Thickness of wall, in inches
        public double WallLength { get; set; } //Length of wall, in inches
    }
}
