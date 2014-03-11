using System;
using System.Collections.Generic;
using Lucene.Net.Documents;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearchResult : ISearchResult, IEnumerable<Document>
	{
		/// <summary>
		/// Gets the top documents returned from the query in the natural lucene scoring order.
		/// </summary>
		/// <returns></returns>
		List<Document> GetTopItems();

        /// <summary>
        /// Gets a range of documents from the index - zero based index
        /// </summary>
        /// <param name="start">The start index to use</param>
        /// <param name="end">The last document to use</param>
        /// <returns></returns>
        List<Document> GetRange(int start, int end);

		/// <summary>
		/// Gets a paged set of documents from the index - zero based index
		/// </summary>
		/// <param name="start">The start index to use</param>
		/// <param name="end">The last document to use</param>
		/// <returns></returns>
        [Obsolete("GetPagedItems is being deprecated, use GetRange instead")]
        List<Document> GetPagedItems(int start, int end);
	}
}
