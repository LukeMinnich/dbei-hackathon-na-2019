using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kataclysm.Common
{
    public static class XFile
    {
        public static List<string[]> ReadCSV(string path, bool hasHeaderRow, out Dictionary<string, int> headerLookup,
            bool ignoreHeaderCase)
        {
            string[] lines = File.ReadAllLines(path);

            int startIndex;
            
            if (hasHeaderRow)
            {
                startIndex = 1;
                
                var headers = lines[0].Split(',').Select(CleanUpCSVRowItem).ToArray();

                var headerCaseSensitivity = ignoreHeaderCase
                    ? StringComparer.InvariantCultureIgnoreCase
                    : StringComparer.InvariantCulture;
                
                headerLookup = new Dictionary<string, int>(headerCaseSensitivity);

                for (var i = 0; i < headers.Length; i++)
                {
                    headerLookup.Add(headers[i], i);
                }
            }
            else
            {
                startIndex = 0;
                headerLookup = null;
            }

            var rows = new List<string[]>();

            for (var i = startIndex; i < lines.Length; i++)
            {
                rows.Add(lines[i].Split(',').Select(CleanUpCSVRowItem).ToArray());
            }

            return rows;
        }

        // Trims off surrounding spaces and double quotes, then replaces "" with "
        //   since Excel likes to add all the extra quotes when generating CSVs
        private static readonly Func<string, string> CleanUpCSVRowItem = s => s.Trim().Trim('\"').Replace("\"\"", "\"");
    }
}