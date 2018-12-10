using Lucene.Net.Search;
using Lucinq.Core.Enums;

namespace Lucinq.Lucene30.Extensions
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
    }
}
