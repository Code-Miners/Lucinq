using Lucene.Net.Search;

namespace Lucinq.Enums
{
    public enum Equality
    {
        NotSet,
        Always = Occur.MUST,
        Sometimes = Occur.SHOULD,
        Never = Occur.MUST_NOT
    }
}
