using System.Collections.Generic;
using Kataclysm.Common;
using Katerra.Apollo.Structures.Common.Units;

namespace Kataclysm.StructuralAnalysis.Model
{
    public struct ModelSettings
    {
        public UnitSystem UnitSystem { get; set; }
        public List<BuildingLevel> BuildingLevels { get; set; }
        public BuildingLevel BaseLevel { get; set; }
        public double BuildingHeight { get; set; }

        public ModelSettings(UnitSystem unitSystem, List<BuildingLevel> buildingLevels, BuildingLevel baseLevel,
            double buildingHeight)
        {
            UnitSystem = unitSystem;
            BuildingLevels = buildingLevels;
            BaseLevel = baseLevel;
            BuildingHeight = buildingHeight;
        }
    }
}