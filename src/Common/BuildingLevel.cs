using System;
using Kataclysm.Common.Extensions;

namespace Kataclysm.Common
{
    public class BuildingLevel : IEquatable<BuildingLevel>
    {
        public string Name { get; }
        public double Elevation { get; }

        public BuildingLevel(string name, double elevation)
        {
            Name = name;
            Elevation = elevation;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BuildingLevel);
        }

        public bool Equals(BuildingLevel obj)
        {
            return obj != null && this.Name == obj.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return $"Level {Name.Replace("Level", "", StringComparison.InvariantCultureIgnoreCase)}";
        }
    }
}