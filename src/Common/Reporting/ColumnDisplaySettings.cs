namespace Kataclysm.Common.Reporting
{
    public abstract class ColumnDisplaySettings
    {
        public string Name { get; }
        public string HeaderName { get; }
        public Alignment Alignment { get; set; } = Alignment.Center;

        protected ColumnDisplaySettings(string name, string headerName)
        {
            Name = name;
            HeaderName = headerName;
        }
    }
}