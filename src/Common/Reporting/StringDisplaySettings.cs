using Kataclysm.Common.Reporting;

namespace Katerra.Apollo.Structures.Common.Reporting.Tabular
{
    public class StringDisplaySettings : ColumnDisplaySettings
    {
        public StringCase Case { get; set; } = StringCase.Unchanged;

        public StringDisplaySettings(string name, string headerName)
            : base(name, headerName)
        {
        }
    }
}