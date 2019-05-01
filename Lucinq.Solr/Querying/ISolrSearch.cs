namespace Lucinq.Solr.Querying
{
    using System;
    using Core.Interfaces;
    using Core.Querying;

    public interface ISolrSearch<out T> where T : class, ISearchResult
	{
		T Execute(LucinqQueryModel lucinqModel, int noOfResults);

		T Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1);

	    void Collect(LucinqQueryModel model);
	}
}
