using System;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
    public class RamDirectorySearcherProvider : IIndexSearcherProvider
    {
        public RamDirectorySearcherProvider(string indexPath)
        {
            var fileSystemDirectory = FSDirectory.Open(indexPath);
            var ramDirectory = new RAMDirectory(fileSystemDirectory);

            IndexSearcher = new IndexSearcher(ramDirectory);
        }

        public void Dispose()
        {
            IndexSearcher.Dispose();
        }

        public IndexSearcher IndexSearcher { get; private set; }
    }
}
