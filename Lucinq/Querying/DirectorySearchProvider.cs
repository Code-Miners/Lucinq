using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
	public class DirectorySearchProvider : IIndexSearcherProvider
	{
	    private IndexSearcher indexSearcher;

	    private readonly Directory currentDirectory;

		public DirectorySearchProvider(Directory luceneDirectory)
		{
			currentDirectory = luceneDirectory;
		}

		public void Dispose()
		{
		    if (indexSearcher == null)
		    {
		        return;
		    }
			indexSearcher.Dispose();
		    indexSearcher = null;
		    // Cannot dispose of this, as it doesn't work.
		    //CurrentDirectory.Dispose();
		}

	    public IndexSearcher IndexSearcher
	    {
	        get { return indexSearcher ?? (indexSearcher = new IndexSearcher(currentDirectory)); }
	    }
	}
}
