using System.Collections.Generic;
using System.Linq;

namespace Kataclysm.Common
{
    public class LevelDataDictionary<T> : Dictionary<BuildingLevel, T> where T : IReportsLevel
    {
        public void Add(T obj)
        {
            var level = obj.Level;

            if (!this.ContainsKey(level))
            {
                this.Add(level, obj);
                return;
            }

            this[level] = obj;
        }

        public List<T> DataOrderedFromBottomToTopLevel()
        {
            return this.Select(d => d.Value).OrderBy(t => t.Level.Elevation).ToList();
        }

        public List<T> DataOrderedFromTopToBottomLevel()
        {
            return this.Select(d => d.Value).OrderByDescending(t => t.Level.Elevation).ToList();
        }
    }
}