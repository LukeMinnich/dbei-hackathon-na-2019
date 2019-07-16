namespace Kataclysm.Common.Reporting
{
    public class EquationDisplayParameters
    {
        public bool IncludeMathematicalEquation { get; set; }
        public bool IncludeInlineCalculation { get; set; }
        public bool IncludeResult { get; set; }
        public bool IncludeEquationNumber { get; set; }
        public bool IncludeComparison { get; set; }
        public bool IncludeOverride { get; set; }

        public static EquationDisplayParameters EquationDisplay => new EquationDisplayParameters()
        {
            IncludeMathematicalEquation = true,
            IncludeInlineCalculation = false,
            IncludeResult = false,
            IncludeEquationNumber = true,
            IncludeComparison = true,
            IncludeOverride = false
        };
        
        public static EquationDisplayParameters CalculationDisplay => new EquationDisplayParameters()
        {
            IncludeMathematicalEquation = false,
            IncludeInlineCalculation = true,
            IncludeResult = true,
            IncludeEquationNumber = false,
            IncludeComparison = true,
            IncludeOverride = true
        };

        public static EquationDisplayParameters CalculationResult => new EquationDisplayParameters()
        {
            IncludeMathematicalEquation = false,
            IncludeInlineCalculation = false,
            IncludeResult = true,
            IncludeEquationNumber = false,
            IncludeComparison = false,
            IncludeOverride = false
        };

        public static EquationDisplayParameters DisplayAll => new EquationDisplayParameters()
        {
            IncludeMathematicalEquation = true,
            IncludeInlineCalculation = true,
            IncludeResult = true,
            IncludeEquationNumber = true,
            IncludeComparison = true,
            IncludeOverride = true
        };
    }
}