using Lucene.Net.Search;
using Lucinq.Enums;
using Lucene.Net.Documents;

namespace Lucinq.Extensions
{
    public static class ConversionExtensions
    {
        public static Occur GetLuceneOccurance(this Matches matches)
        {
            switch (matches)
            {
                case Matches.Never:
                    return Occur.MUST_NOT;
                case Matches.Sometimes:
                    return Occur.SHOULD;
                default:
                    return Occur.MUST;
            }
        }

        public static Field.Store GetLuceneStorage(this Store store)
        {
            switch (store)
            {
                case Store.Yes:
                    return Field.Store.YES;
                case Store.Compress:
                    return Field.Store.YES;
                default:
                    return Field.Store.NO;
            }
        }

        #region [ Api Compatibility Conversions]

        public static void SetSlop(this PhraseQuery phraseQuery, int slop)
        {
            phraseQuery.Slop = slop;
        }

        public static void SetBoost(this Query query, float boost)
        {
            query.Boost = boost;
        }

        #endregion
    }
}
