namespace Lucinq.Core.Querying
{
    public class LucinqRangeQuery<T> : LucinqQuery
    {
        public LucinqRangeQuery(string fieldName, int? precisionStep, T lower, T upper, bool includeMin, bool includeMax, string adapterToUse) : this(fieldName, lower, upper, includeMin, includeMax, adapterToUse)
        {
            PrecisionStep = precisionStep;
        }

        public LucinqRangeQuery(string fieldName, T lower, T upper, bool includeMin, bool includeMax, string adapterToUse)
        {
            Field = fieldName;
            Lower = lower;
            Upper = upper;
            IncludeMin = includeMin;
            IncludeMax = includeMax;
            AdapterToUse = adapterToUse;
        }

        public string Field { get; set; }

        public bool IncludeMax { get; set; }

        public bool IncludeMin { get; set; }

        public int? PrecisionStep { get; set; }

        public T Lower { get; set; }

        public T Upper { get; set; }

        protected string AdapterToUse { get; }
    }
}
