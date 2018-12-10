using System;
using Lucene.Net.Search;

namespace Lucinq.Lucene30.Querying
{
    public interface IIndexSearcherProvider : IDisposable
    {
        IndexSearcher IndexSearcher { get; }

        bool ClosesDirectory { get; }
    }
}
