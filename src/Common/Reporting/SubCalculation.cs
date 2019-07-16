using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Kataclysm.Common.Units.Conversion;
using Katerra.Apollo.Structures.Common.Units;
using Newtonsoft.Json;

namespace Kataclysm.Common.Reporting
{
    public class SubCalculation
    {
//        public ReferenceStandard CodeOrStandard { get; set; }
        public string Description { get; set; }
        public string Chapter { get; set; }
        public string SectionNumber { get; set; }
        public string EquationNumber { get; set; }
        public string LaTexOutputMoniker { get; set; }
        public string LaTexExpression { get; set; }
        public bool SplitOntoTwoLines { get; set; }
        public bool SplitOntoThreeLines { get; set; }

        private Result _result;

        public Result Result
        {
            get
            {
                if (Limit == null)
                {
                    return _result;
                }

                switch (Limit.Type)
                {
                    case ResultLimitType.GreaterThan:
                    case ResultLimitType.GreaterThanOrEqual:
                    case ResultLimitType.MaximumOf:
                        return Result.Max(_result, Limit.Result);
                    case ResultLimitType.LessThan:
                    case ResultLimitType.LessThanOrEqual:
                    case ResultLimitType.MinimumOf:
                        return Result.Min(_result, Limit.Result);
                    case ResultLimitType.Override:
                        return Limit.Result;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }

            set => _result = value;
        }
        public ResultLimit Limit { get; set; }

        private List<string> _notes { get; set; } = new List<string>();
        private Dictionary<string, LaTexVariable> _variables { get; } = new Dictionary<string, LaTexVariable>();

        public IReadOnlyList<string> Notes => new ReadOnlyCollection<string>(_notes);

        [JsonConstructor]
        public SubCalculation(ReferenceStandard standard, string description, string chapter, string sectionNumber,
            string equationNumber, string laTexExpression, string laTexOutputMoniker)
        {
//            CodeOrStandard = standard;
            Description = description;
            Chapter = chapter;
            SectionNumber = sectionNumber;
            EquationNumber = equationNumber;
            LaTexExpression = laTexExpression;
            LaTexOutputMoniker = laTexOutputMoniker;
        }

        public SubCalculation(ReferenceStandard standard, string description, string chapter, string sectionNumber,
            string equationNumber, string laTexExpression, string laTexOutputMoniker, bool displayOnTwoLines)
            : this(standard, description, chapter, sectionNumber, equationNumber, laTexExpression, laTexOutputMoniker)
        {
            SplitOntoTwoLines = displayOnTwoLines;
        }

        public void AddVariable(string laTexMoniker, double value)
        {
            AddVariable(new LaTexVariable(laTexMoniker, value));
        }

        public void AddVariable(string laTexMoniker, Result result, Unit displayUnit)
        {
           AddVariable(new LaTexVariable(laTexMoniker, result, displayUnit));
        }

        public void AddVariable(LaTexVariable variable)
        {
            _variables.Add(variable.LaTexMoniker, variable);
        }

        public void AddNote(string note)
        {
            _notes.Add(note);
        }

        public void AddNote(List<string> notes)
        {
            _notes.AddRange(notes);
        }

        public string GenerateMarkdownWithLaTex(EquationDisplayParameters displayParameters, bool inline, MarkdownConverterType converter)
        {
            switch (converter)
            {
                case MarkdownConverterType.Markdig:
                    return GenerateMarkdownWithLaTex_Markdig(displayParameters, inline);
                case MarkdownConverterType.Pandoc:
                    return GenerateMarkdownWithLaTex_Pandoc(displayParameters, inline);
                default:
                    return "";
            }
        }

        public string GenerateMarkdownWithLaTex_Pandoc(EquationDisplayParameters displayParameters, bool inline)
        {
            bool includeEquation = displayParameters.IncludeMathematicalEquation;
            bool includeCalculation = displayParameters.IncludeInlineCalculation;
            bool includeResult = displayParameters.IncludeResult;
            bool includeComparison = displayParameters.IncludeComparison;
            bool includeOverride = displayParameters.IncludeOverride;

            var sB = new StringBuilder();

            var laTexInlineCalculation = ConvertToLaTexInlineCalculation(LaTexExpression, _variables.Values);

            sB.Append(inline ? "$" : "$$\n");
            
            sB.Append($"{LaTexOutputMoniker} = ");

            if (includeEquation && LaTexExpression!="") sB.Append(LaTexExpression);

            if (includeEquation && (includeCalculation || includeResult) && LaTexExpression!="") sB.Append(" = ");

            if (includeCalculation && laTexInlineCalculation!="") sB.Append(laTexInlineCalculation);

            if (includeCalculation && includeResult && laTexInlineCalculation != "") sB.Append(" = ");

            if (includeResult == true && (includeEquation == false && includeCalculation == false && includeComparison == false && includeOverride == false))
            {
                sB.Append($@"{Result.ToLaTexString()}");
            }
            else
            {
                if (includeResult) sB.Append($@"{_result.ToLaTexString()}");
            }

            if (includeComparison && includeCalculation == false) sB.Append(ResultLimitComparison());
            if (includeComparison && includeCalculation)
            {
                sB.Append(ConvertToLaTexInlineCalculation(ResultLimitComparison(), _variables.Values));
                if (Limit != null)
                {
                    sB.Append(" = " + Result.ToLaTexString());
                }
            }

            if (includeOverride) sB.Append(ResultLimitOverride());

            if (displayParameters.IncludeEquationNumber && !string.IsNullOrWhiteSpace(EquationNumber))
                sB.Append($@"\tag{{{EquationNumber} }}");

            sB.Append(inline ? "$" : "\n$$");

            sB.Append($"\n\n");

            return sB.ToString();
        }


        public string GenerateMarkdownWithLaTex_Markdig(EquationDisplayParameters displayParameters, bool inline)
        {
            bool includeEquation = displayParameters.IncludeMathematicalEquation;
            bool includeCalculation = displayParameters.IncludeInlineCalculation;
            bool includeResult = displayParameters.IncludeResult;
            bool includeComparison = displayParameters.IncludeComparison;
            bool includeOverride = displayParameters.IncludeOverride;

            var sB = new StringBuilder();

            var laTexInlineCalculation = ConvertToLaTexInlineCalculation(LaTexExpression, _variables.Values);

            sB.Append(inline ? "$" : "$$\n");

            sB.Append(@"\begin{equation}");
            sB.Append("\n");

            sB.Append($"{LaTexOutputMoniker} = ");

            if (includeEquation) sB.Append(LaTexExpression);

            if (includeEquation && (includeCalculation || includeResult)) sB.Append(" = ");

            if (includeCalculation) sB.Append(laTexInlineCalculation);

            if (includeCalculation && includeResult) sB.Append(" = ");

            if (includeResult) sB.Append($@"{_result.ToLaTexString()}");

            if (includeComparison) sB.Append(ResultLimitComparison());

            if (includeOverride) sB.Append(ResultLimitOverride());

            if (displayParameters.IncludeEquationNumber && !string.IsNullOrWhiteSpace(EquationNumber))
                sB.Append($@"\tag{{{EquationNumber} }}");

            sB.Append("\n");
            sB.Append(@"\end{equation}");


            sB.Append(inline ? "$" : "\n$$");

            sB.Append($"\n\n");

            return sB.ToString();
        }


        internal static string ConvertToLaTexInlineCalculation(string laTexExpression,
            IEnumerable<LaTexVariable> variables)
        {
            string laTexInlineCalculation = laTexExpression;

            foreach (var variable in variables)
            {
                string formattedAndConvertedValue = variable.ToInlineString();

                string substitution = $@"{formattedAndConvertedValue}";

                laTexInlineCalculation =
                    ReplaceLaTexMoniker(laTexInlineCalculation, variable.LaTexMoniker, substitution);
            }

            return laTexInlineCalculation;
        }

        private static string ReplaceLaTexMoniker(string source, string laTexMoniker, string replacement)
        {
            // escape backslashes
            var modifiedMoniker = laTexMoniker.Replace("\\", @"\\");

            // escape caret symbols
            modifiedMoniker = modifiedMoniker.Replace("^", @"\^");

            // escape asterisks
            modifiedMoniker = modifiedMoniker.Replace("*", @"\*");
            
            // escape plus symbol
            modifiedMoniker = modifiedMoniker.Replace("+", @"\+");

            var replacementWithParentheses = "(" + replacement + ")";

            var patternForParenthesesReplacement =
                @"\b" + modifiedMoniker + @"(?=[\s\^][\w\(\\])|(?<=[\)\w]\s)" + modifiedMoniker;

            var result = Regex.Replace(source, patternForParenthesesReplacement, replacementWithParentheses);

            var patternNoParenthesesReplacement = @"(?<!\w)" + modifiedMoniker + @"(?=$)" +
                                                  @"|(?<!\w)" + modifiedMoniker + @"(?!\w)" +
                                                  @"|(?<=^)" + modifiedMoniker + @"(?!\w)";

            result = Regex.Replace(result, patternNoParenthesesReplacement, replacement);

            return result;
        }

        private string ResultLimitComparison()
        {
            return (Limit == null)
                ? string.Empty
                : Limit.LaTexComparison();
        }

        private string ResultLimitOverride()
        {
            if (Limit == null) return string.Empty;

            switch (Limit.Type)
            {
                case ResultLimitType.GreaterThan:
                case ResultLimitType.GreaterThanOrEqual:
                case ResultLimitType.LessThan:
                case ResultLimitType.LessThanOrEqual:
                    return string.Empty;

                case ResultLimitType.Override:
                    return $@" \rightarrow {Limit}";

                case ResultLimitType.MaximumOf:
                case ResultLimitType.MinimumOf:
                    return Math.Abs(Result.Value - _result.Value) < 1e-10
                        ? string.Empty
                        : $@" \Rightarrow {LaTexOutputMoniker} = {Limit.ToInlineString()}";

                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}