using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kataclysm.Common
{
    public class LevelDictionary<T> : Dictionary<BuildingLevel, List<T>> where T : IReportsLevel
    {
        public LevelDictionary() {}
        
        public LevelDictionary(IEnumerable<T> objects)
        {
            AddRange(objects);
        }

        public IReadOnlyList<BuildingLevel> Levels => new ReadOnlyCollection<BuildingLevel>(this.Keys.ToList());

        public void AddRange(IEnumerable<T> objects)
        {
            if (objects == null || !objects.Any()) return;

            var groupedByLevel = objects.GroupBy(o => o.Level);

            foreach (IGrouping<BuildingLevel,T> grouping in groupedByLevel)
            {
                var level = grouping.Key;
                
                AddLevelIfNonexistant(level);
                
                this[level].AddRange(grouping);
            }
        }

        private void AddLevelIfNonexistant(BuildingLevel level)
        {
            if (!this.ContainsKey(level))
            {
                this.Add(level, new List<T>());
            }
        }
    }
}
