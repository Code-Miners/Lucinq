using Lucinq.Core.Enums;

namespace Lucinq.Core.Querying
{
    public abstract class LucinqQuery
    {
        public float? Boost { get; set; }

        public Matches Matches { get; set; }
    }
}
