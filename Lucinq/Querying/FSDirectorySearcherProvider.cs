using System;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
    public class FSDirectorySearcherProvider : IIndexSearcherProvider
    {
        private readonly FSDirectory fileSystemDirectory;

        public FSDirectorySearcherProvider(string indexPath)
        {
            fileSystemDirectory = FSDirectory.Open(indexPath);
            IndexSearcher = new IndexSearcher(fileSystemDirectory);
        }

        public void Dispose()
        {
            IndexSearcher.Dispose();
            fileSystemDirectory.Dispose();
        }

        public IndexSearcher IndexSearcher { get; private set; }
    }
}
