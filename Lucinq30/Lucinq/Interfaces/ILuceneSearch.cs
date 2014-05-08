using System;
using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearch<out T> where T : class, ISearchResult
	{
		T Execute(Query query, int noOfResults, Sort sort = null, Filter filter = null);

		T Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue);

	    void Collect(Query query, Collector customCollector, Filter filter = null);
	}
}
