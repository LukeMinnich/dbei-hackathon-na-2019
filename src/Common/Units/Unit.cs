using Kataclysm.Common;

namespace Katerra.Apollo.Structures.Common.Units
{
    public abstract class Unit : Enumeration
    {
        protected Unit(int value, string description)
            : base(value, description)
        {
        }
    }
}