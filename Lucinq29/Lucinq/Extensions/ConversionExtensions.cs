using Lucene.Net.Search;
using Lucinq.Enums;
using Lucene.Net.Documents;

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

        public static Field.Store GetLuceneStorage(this Store store)
        {
            switch (store)
            {
                case Store.Yes:
                    return Field.Store.YES;
                case Store.Compress:
                    return Field.Store.COMPRESS;
                default:
                    return Field.Store.NO;
            }
        }
    }
}
