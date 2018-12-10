namespace Lucinq.Core.Querying
{
    public class LucinqFuzzyQuery : LucinqTermQuery
    {
        public float MinSimilarity { get; }

        public LucinqFuzzyQuery(LucinqTerm lucinqTerm, float minSimilarity) : base(lucinqTerm)
        {
            MinSimilarity = minSimilarity;
        }
    }
}
