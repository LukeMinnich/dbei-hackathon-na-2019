using System.Collections.Generic;
using System.Linq;
using Kataclysm.Common;
using Kataclysm.Common.Units.Conversion;
using Katerra.Apollo.Structures.Common.Units;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    internal class LevelHeightResolver
    {
        private List<BuildingLevelLateral2> _levels;
        private BuildingLevel _baseLevel;
        
        public LevelHeightResolver(IEnumerable<BuildingLevelLateral2> levels, BuildingLevel baseLevel)
        {
            _levels = levels.ToList();
            _baseLevel = baseLevel;
        }

        public void Resolve()
        {
            List<BuildingLevelLateral2> levels = _levels.Where(l => l.Level.Elevation > _baseLevel.Elevation)
                .OrderByDescending(l => l.Level.Elevation).ToList();
            
            var lastIndex = levels.Count - 1;

            for (var i = 0; i < lastIndex; i++)
            {
                var thisLevelElevation = new Length(levels[i].Level.Elevation, LengthUnit.Inch);
                var belowLevelElevation = new Length(levels[i + 1].Level.Elevation, LengthUnit.Inch);

                levels[i].Height = (Length) (thisLevelElevation - belowLevelElevation);
            }

            levels[lastIndex].Height = (Length) (new Length(levels[lastIndex].Level.Elevation, LengthUnit.Inch) -
                                                 new Length(_baseLevel.Elevation, LengthUnit.Inch));
        }
    }
}