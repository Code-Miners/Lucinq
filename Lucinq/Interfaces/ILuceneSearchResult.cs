using System.Collections.Generic;
using Lucene.Net.Documents;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearchResult<T> : ISearchResult, IEnumerable<T>
	{
		/// <summary>
		/// Gets the top documents returned from the query in the natural lucene scoring order.
		/// </summary>
		/// <returns></returns>
		List<T> GetTopItems();

		/// <summary>
		/// Gets a paged set of documents from the index - zero based index
		/// </summary>
		/// <param name="start">The start index to use</param>
		/// <param name="end">The last document to use</param>
		/// <returns></returns>
		List<T> GetPagedItems(int start, int end);
	}
}
