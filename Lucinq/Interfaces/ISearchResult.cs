using System.Collections.Generic;
using Lucene.Net.Documents;

namespace Lucinq.Interfaces
{
	public interface ISearchResultBase<out T>
	{
		int TotalHits { get; }

		T Results { get; }

		List<Document> GetTopDocuments();
	}
}
