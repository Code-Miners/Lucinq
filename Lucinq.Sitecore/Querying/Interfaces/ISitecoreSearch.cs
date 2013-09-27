using System;
using Lucene.Net.Search;
using Lucinq.Interfaces;
using Lucinq.Querying;

namespace Lucinq.SitecoreIntegration.Querying.Interfaces
{
	public interface ISitecoreSearch
	{
		/// <summary>
		/// Gets the lucinq lucene search object
		/// </summary>
		ILuceneSearch<LuceneSearchResult> LuceneSearch { get; }

		ISitecoreSearchResult Execute(Query query, int noOfResults = Int32.MaxValue - 1, Sort sort = null);
		ISitecoreSearchResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1, Sort sort = null);
	}
}