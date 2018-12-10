using System;
using Lucinq.Core.Interfaces;
using Lucinq.Core.Querying;

namespace Lucinq.AzureSearch.Querying
{
	public interface IAzureSearch<out T> where T : class, ISearchResult
	{
		T Execute(LucinqQueryModel lucinqModel, int noOfResults);

		T Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1);

	    void Collect(LucinqQueryModel model);
	}
}
