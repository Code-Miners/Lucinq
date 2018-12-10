namespace Lucinq.Core.Querying
{
    public class LucinqPhraseQuery : LucinqQuery
    {
        public LucinqTerm[] LucinqTerms { get; }

        public int Slop { get; set; }

        public LucinqPhraseQuery(LucinqTerm[] lucinqTerms)
        {
            this.LucinqTerms = lucinqTerms;
        }
    }
}
