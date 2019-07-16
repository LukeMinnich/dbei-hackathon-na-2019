namespace Kataclysm.Common.Reporting
{
    public class NumericDisplaySettings : ColumnDisplaySettings
    {
        public int DecimalPrecision { get; set; }

        public NumericDisplaySettings(string name, string headerName, int decimalPrecision)
            : base(name, headerName)
        {
            DecimalPrecision = decimalPrecision;
        }
    }
}