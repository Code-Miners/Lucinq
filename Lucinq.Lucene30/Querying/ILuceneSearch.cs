using System;
using Lucene.Net.Search;
using Lucinq.Core.Interfaces;
using Lucinq.Core.Querying;

namespace Lucinq.Lucene30.Querying
{
	public interface ILuceneSearch<out T> where T : class, ISearchResult
	{
		T Execute(LucinqQueryModel query, int noOfResults);

		T Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1);

	    void Collect(LucinqQueryModel model, Collector customCollector);
	}
}
