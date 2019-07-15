using Kataclysm.Common.Units.Conversion;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class LevelStiffnessParameters
    {
        public ForcePerLength X { get; set; }
        public ForcePerLength Y { get; set; }
        public Moment J { get; set; }
    }
}