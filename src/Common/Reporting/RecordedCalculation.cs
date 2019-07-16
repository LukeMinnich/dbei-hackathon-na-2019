using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Kataclysm.Common.Extensions;
using Newtonsoft.Json;

namespace Kataclysm.Common.Reporting
{
    public class RecordedCalculation
    {
        public string Header { get; set; }
        public CalculationType Type { get; set; }

        [JsonProperty]
        public List<SubCalculation> _subCalculations = new List<SubCalculation>();

        public RecordedCalculation(string header, CalculationType type)
        {
            Header = header;
            Type = type;
        }

        public IReadOnlyList<SubCalculation> SubCalculations =>
            new ReadOnlyCollection<SubCalculation>(_subCalculations);

        public void AddCalculation(SubCalculation calculation)
        {
            _subCalculations.Add(calculation);
        }

        public string PrintAllResults(EquationDisplayParameters displayParameters, bool inline, MarkdownConverterType converter)
        {
            var sB = new StringBuilder();

            sB.Append("\n<ul>\n");
            sB.Append("<li>\n\n");
            sB.Append(@"#### " + Header);
            sB.Append("\n\n");

            foreach (var subCalculation in _subCalculations)
            {
                sB.Append("\n\n");
                sB.Append(subCalculation.Description);
                sB.Append("\n\n");

                sB.Append("\n<ul>");
                sB.Append("\n<li>\n");
                if (subCalculation.Notes.Count > 0)
                {
                    foreach (string s in subCalculation.Notes)
                    {
                        sB.Append("\n\n");
                        sB.Append(s);
                    }

                }

                sB.Append("\n\n");
                
                if (subCalculation.SplitOntoTwoLines == true)
                {
                    sB.Append(subCalculation.GenerateMarkdownWithLaTex(EquationDisplayParameters.EquationDisplay,
                        inline, converter));
                    sB.Append(subCalculation.GenerateMarkdownWithLaTex(EquationDisplayParameters.CalculationDisplay,
                        inline, converter));
                }
                else if(subCalculation.SplitOntoThreeLines == true)
                {
                    sB.Append(subCalculation.GenerateMarkdownWithLaTex(EquationDisplayParameters.EquationDisplay,
                       inline, converter));
                    sB.Append(subCalculation.GenerateMarkdownWithLaTex(EquationDisplayParameters.CalculationDisplay,
                        inline, converter));
                    sB.Append(subCalculation.GenerateMarkdownWithLaTex(EquationDisplayParameters.CalculationResult,
                        inline, converter));
                }
                else
                {                    
                    sB.Append(subCalculation.GenerateMarkdownWithLaTex(displayParameters, inline, converter));
                }



                sB.Append("\n</li>");
                sB.Append("\n</ul>");
            }
            sB.Append("\n</li>");
            sB.Append("\n</ul>");
            return sB.ToString();
        }

        protected string PrepInlineMathForMarkdig(string s)
        {
            string retString = s;

            string inlineMarker = "$";

            List<int> dollars = s.AllIndexesOf(inlineMarker);

            if (dollars.Count % 2 != 0)
            {
                throw new System.Exception($"There should always be an even number of $ symbols in math expressions.  Found {dollars.Count}.");
            }
            else
            {
                for (int i = 0; i<= dollars.Count - 2; i += 2)
                {
                    int ind1 = dollars[i];
                    int ind2 = dollars[i + 1];

                    string begin = "";// @"\begin{equation}";
                    string end = "";// @"\end{equation}";

                    if (ind2 != ind1 + 1)
                    {
                        retString = retString.Insert(ind1 + 1, begin);
                    }

                    ind2 += begin.Length;

                    retString = retString.Insert(ind2, end);

                    for (int j = i + 1; j <= dollars.Count - 1; j++)
                    {
                        dollars[j] = dollars[j] + begin.Length + end.Length;
                    }
                }
            }

            return retString;
        }
    }
}