using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
	public class DirectorySearchProvider : IIndexSearcherProvider
	{
		private Directory CurrentDirectory { get; set; }

		public DirectorySearchProvider(Directory luceneDirectory)
		{
			CurrentDirectory = luceneDirectory;
			IndexSearcher = new IndexSearcher(luceneDirectory);
		}

		public void Dispose()
		{
			IndexSearcher.Dispose();
            // Cannot dispose of this, as it doesn't work.
			//CurrentDirectory.Dispose();
		}

		public IndexSearcher IndexSearcher { get; private set; }
	}
}
