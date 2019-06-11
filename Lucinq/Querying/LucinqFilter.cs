using Lucinq.Core.Enums;

namespace Lucinq.Core.Querying
{
    public class LucinqFilter
    {
        public string Field { get; }

        public string Value { get; }

        public Comparator Comparator { get; set; }

        public LucinqFilter(string field, string value, Comparator comparator)
        {
            Field = field;
            Value = value;
            Comparator = comparator;
        }
    }
}
