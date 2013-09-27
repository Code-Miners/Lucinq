using System;
using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
    public interface IIndexSearcherProvider : IDisposable
    {
        IndexSearcher IndexSearcher { get; }
    }
}
