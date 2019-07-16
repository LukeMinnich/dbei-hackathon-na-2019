using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Kataclysm.Common.Extensions;

namespace Kataclysm.Common
{
    public static class CommonResources
    {
        public static Dictionary<string, LoadCase> ASCE7LoadCases { get; }

        static CommonResources()
        {
            ASCE7LoadCases = ReadASCE7Cases();
        }

        internal static Dictionary<string, LoadCase> ReadASCE7Cases()
        {
            string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(executingPath, @"Common\ASCE7LoadCases.csv");

            List<string[]> rows = XFile.ReadCSV(path, true, out _, false);

            var cases = new Dictionary<string, LoadCase>(StringComparer.InvariantCultureIgnoreCase);
            
            foreach (string[] row in rows)
            {
                var description = row[0];

                var predominantDirection = (PredominantDirection) Enum.Parse(typeof(PredominantDirection), row[1]);

                var loadPatterns = new List<FactoredLoadPattern>();

                for (var i = 2; i < row.Length; i += 2)
                {
                    var hasData = double.TryParse(row[i], out var factor);

                    if (!hasData) break;
                    
                    LoadPattern pattern = row[i + 1].GetEnumFromDescription<LoadPattern>();
                
                    loadPatterns.Add(new FactoredLoadPattern
                    {
                        Factor = factor,
                        Pattern = pattern
                    });
                }
                
                cases.Add(description, new LoadCase(description, loadPatterns, predominantDirection));
            }

            return cases;
        }

        public static List<LoadCase> ASCE7SeismicELFLoadCases()
        {
            return ASCE7LoadCases.Values.Where(c => c.LoadPatternType == LoadPatternType.Earthquake).ToList();
        }

        public static List<LoadCase> ASCE7WindLoadCases()
        {
            return ASCE7LoadCases.Values.Where(c => c.LoadPatternType == LoadPatternType.Wind).ToList();
        }

        public static List<LoadCase> ASCE7SeismicDiaphragmLoadCases()
        {
            return ASCE7LoadCases.Values.Where(c => c.LoadPatternType == LoadPatternType.Earthquake_Diaphragm).ToList();
        }

        public static List<LoadCase> ASCE7SeismicLoadCases()
        {
            var allSeismic = ASCE7SeismicELFLoadCases();

            allSeismic.AddRange(ASCE7SeismicDiaphragmLoadCases());

            return allSeismic;
        }

        private static bool HasAPatternAtNumber(LoadCase loadCase, int number)
        {
            return loadCase.LoadPatterns.Count >= number;
        } 
    }
}