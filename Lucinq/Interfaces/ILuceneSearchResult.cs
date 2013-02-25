using System.Collections.Generic;
using Lucene.Net.Documents;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearchResult<out T> : ILuceneSearchResult
	{
		/// <summary>
		/// The results returned from the index
		/// </summary>
		T Results { get; }
	}

	public interface ILuceneSearchResult : ISearchResult
	{
		/// <summary>
		/// Gets the top documents returned from the query in the natural lucene scoring order.
		/// </summary>
		/// <returns></returns>
		List<Document> GetTopDocuments();

		/// <summary>
		/// Gets a paged set of documents from the index - zero based index
		/// </summary>
		/// <param name="start">The start index to use</param>
		/// <param name="end">The last document to use</param>
		/// <returns></returns>
		List<Document> GetPagedDocuments(int start, int end);

		/// <summary>
		/// Gets a document from the index
		/// </summary>
		/// <param name="documentId"></param>
		/// <returns></returns>
		Document GetDocument(int documentId);
	}

	public interface ISearchResult
	{
		/// <summary>
		/// The total matches found
		/// </summary>
		int TotalHits { get; }

		/// <summary>
		/// The elapsed time in ms for the query to execute
		/// </summary>
		long ElapsedTimeMs { get; set; }
	}
}
