using Newtonsoft.Json;

namespace Katerra.Apollo.Structures.Common.Units
{
    public class TimeUnit : Unit
    {
        public static readonly TimeUnit Seconds = new TimeUnit(0, "s");

        [JsonConstructor]
        private TimeUnit(int value, string description)
            : base(value, description)
        {
        }
    }
}