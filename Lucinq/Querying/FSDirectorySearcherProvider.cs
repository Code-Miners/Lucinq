using System;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
    public class FSDirectorySearcherProvider : IIndexSearcherProvider
    {
        public FSDirectorySearcherProvider(string indexPath)
        {
            var fileSystemDirectory = FSDirectory.Open(indexPath);
            IndexSearcher = new IndexSearcher(fileSystemDirectory);
        }

        public void Dispose()
        {
            IndexSearcher.Dispose();
        }

        public IndexSearcher IndexSearcher { get; private set; }
    }
}
