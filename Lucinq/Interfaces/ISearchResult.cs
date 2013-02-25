using System.Collections.Generic;
using Lucene.Net.Documents;

namespace Lucinq.Interfaces
{
	public interface ISearchResult<out T> : ISearchResult
	{
		T Results { get; }
	}

	public interface ISearchResult
	{
		int TotalHits { get; }

		List<Document> GetTopDocuments();

		List<Document> GetPagedDocuments(int start, int end);

		Document GetDocument(int documentId);
	}
}
