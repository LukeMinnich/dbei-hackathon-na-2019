using Newtonsoft.Json;

namespace Kataclysm.Common.Reporting
{
    public class SortOrder : Enumeration
    {
        public static readonly SortOrder Ascending = new SortOrder(0, "ASC");
        public static readonly SortOrder Descending = new SortOrder(1, "DESC"); 

        [JsonConstructor]
        private SortOrder(int value, string description)
            : base(value, description)
        {
        }
    }
}