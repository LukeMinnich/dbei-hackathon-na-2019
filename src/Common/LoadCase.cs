using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Kataclysm.Common
{
    public class LoadCase
    {
        public string Description { get; }
        public LoadPatternType LoadPatternType { get; }
        public PredominantDirection PredominantDirection { get; }
        public List<FactoredLoadPattern> LoadPatterns { get; }

        [JsonConstructor]
        public LoadCase(string description, IEnumerable<FactoredLoadPattern> loadPatterns, PredominantDirection direction)
        {
            Description = description;
            LoadPatterns = loadPatterns.ToList();
            LoadPatternType = LoadPatternTypeConverter.Convert(LoadPatterns.First().Pattern);
            PredominantDirection = direction;
            
            ValidateLoadPatternsAllTheSame();
        }

        private void ValidateLoadPatternsAllTheSame()
        {
            if (LoadPatterns.Any(p => LoadPatternTypeConverter.Convert(p.Pattern) != LoadPatternType))
                throw new InvalidDataException("did not expect mismatched load patterns in the same load case");
        }

        public override string ToString()
        {
            return Description;
        }
    }
}