using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Kataclysm.Common
{
    /// <summary>
    /// An unordered collection of element responses, where each item is specific to a single Load Pattern
    /// </summary>
    /// <typeparam name="T">Element response parameter</typeparam>
    public abstract class ResponseCollection<T> : List<T>, IReportsElementId where T : SuperimposableResponse<T>, new()
    {
        public string ElementId { get; }

        [JsonConstructor]
        protected ResponseCollection(IEnumerable<T> objects)
            : this(objects.First().ElementId)
        {
            this.AddRange(objects);
        }

        protected ResponseCollection()
        {
        }

        protected ResponseCollection(string elementId)
        {
            ElementId = elementId;
        }

        public abstract T EnvelopeResponsesAbsolute(IEnumerable<LoadCase> loadCases);
        public abstract Tuple<T, T> EnvelopeResponsesMinMax(IEnumerable<LoadCase> loadCases);

        public T GetResponseAtLoadPattern(LoadPattern loadPattern)
        {
            return this.First(l => l.LoadPattern == loadPattern);
        }

        public IEnumerable<T> GetSuperimposedResponsesAtLoadCases(IEnumerable<LoadCase> loadCases)
        {
            return loadCases.Select(loadCase => SuperimposeResponsesAtLoadCase(loadCase));
        }
        
        public virtual T SuperimposeResponsesAtLoadCase(LoadCase loadCase)
        {
            T superimposedResult = new T();

            superimposedResult.ElementId = ElementId;
            superimposedResult.LoadPattern = LoadPattern.Mixed;
            superimposedResult.LoadCase = loadCase;
            
            if (Count == 0) return superimposedResult;

            Dictionary<LoadPattern, double> loadPatternFactors = GetLoadPatternFactors(loadCase);
            
            foreach (T item in this)
            {
                LoadPattern loadPattern = item.LoadPattern;

                if (!loadPatternFactors.ContainsKey(loadPattern)) continue;

                superimposedResult = superimposedResult.Superimpose(item.ApplyLoadFactor(loadPatternFactors[loadPattern]));
            }

            return superimposedResult;
        }

        private static Dictionary<LoadPattern, double> GetLoadPatternFactors(LoadCase loadCase)
        {
            var factors = new Dictionary<LoadPattern, double>();
            
            loadCase.LoadPatterns.ForEach(p => factors.Add(p.Pattern, p.Factor));

            return factors;
        }
    }
}