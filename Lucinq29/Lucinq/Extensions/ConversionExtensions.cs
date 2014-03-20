using Lucene.Net.Search;
using Lucinq.Enums;

namespace Lucinq.Extensions
{
    public static class ConversionExtensions
    {
        public static BooleanClause.Occur GetLuceneOccurance(this Matches matches)
        {
            switch (matches)
            {
                case Matches.Never:
                    return BooleanClause.Occur.MUST_NOT;
                case Matches.Sometimes:
                    return BooleanClause.Occur.SHOULD;
                default:
                    return BooleanClause.Occur.MUST;
            }
        }
    }
}
