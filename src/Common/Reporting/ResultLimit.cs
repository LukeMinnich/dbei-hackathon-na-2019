using System.ComponentModel;
using Kataclysm.Common.Units.Conversion;

namespace Kataclysm.Common.Reporting
{
    public class ResultLimit : LaTexVariable
    {
        public ResultLimitType Type { get; }

        public ResultLimit(Result result, ResultLimitType type, string expression = "")
            : base(expression, result, result.DisplayUnit)
        {
            Type = type;
        }

        public string LaTexComparison()
        {
            switch (Type)
            {
                case ResultLimitType.GreaterThan:
                    return $@" \gt {ToInlineString()}";

                case ResultLimitType.GreaterThanOrEqual:
                    return $@" \geq {ToInlineString()}";

                case ResultLimitType.LessThan:
                    return $@" \lt {ToInlineString()}";

                case ResultLimitType.LessThanOrEqual:
                    return $@" \leq {ToInlineString()}";

                case ResultLimitType.Override:
                    return $@" \rightarrow {ToInlineString()}";

                case ResultLimitType.MaximumOf:
                    return $@" \geq {ToInlineString()}";

                case ResultLimitType.MinimumOf:
                    return $@" \leq {ToInlineString()}";

                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public new string ToInlineString()
        {
            return string.IsNullOrEmpty(LaTexMoniker)
                ? Result.ToLaTexString()
                : LaTexMoniker;
        }
    }
}