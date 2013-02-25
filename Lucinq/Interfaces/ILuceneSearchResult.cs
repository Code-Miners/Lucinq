using System.Collections.Generic;
using Lucene.Net.Documents;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearchResult<out T> : ILuceneSearchResult
	{
		T Results { get; }
	}

	public interface ILuceneSearchResult : ISearchResult
	{
		List<Document> GetTopDocuments();

		List<Document> GetPagedDocuments(int start, int end);

		Document GetDocument(int documentId);
	}

	public interface ISearchResult
	{
		int TotalHits { get; }	
	}
}
