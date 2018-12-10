namespace Lucinq.Core.Querying
{
    public class LucinqTermQuery : LucinqQuery
    {
        public LucinqTermQuery(LucinqTerm searchTerm)
        {
            SearchTerm = searchTerm;
        }

        public LucinqTerm SearchTerm { get; set; }
    }
}
