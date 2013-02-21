using System;
using System.IO;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Interfaces;

namespace Lucinq
{
	public class LuceneSearch : ILuceneSearch<LuceneSearchResult>, IDisposable
	{
		#region [ Constructors ]

		public LuceneSearch(string indexPath)
		{
			IndexSearcher = new IndexSearcher(FSDirectory.Open(new DirectoryInfo(indexPath)), true);
		}

		#endregion

		#region [ Properties ]

		public IndexSearcher IndexSearcher { get; private set; }

		#endregion

		#region [ Methods ]

		public LuceneSearchResult Execute(Query query, int noOfResults)
		{
			TopDocs topDocs = IndexSearcher.Search(query, null, noOfResults);
			LuceneSearchResult searchResult = new LuceneSearchResult(this, topDocs);
			return searchResult;
		}

		public LuceneSearchResult Execute(IQueryBuilder queryBuilder, int noOfResults)
		{
			return Execute(queryBuilder.Build(), noOfResults);
		}

		#endregion

		public void Dispose()
		{
			IndexSearcher.Dispose();
		}
	}
}
