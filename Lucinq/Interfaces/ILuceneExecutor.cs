using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearchExecutor<T> where T : class, ISearchResult
	{
		T Execute(Query query, int noOfResults, Sort sort = null);

		T Execute(IQueryBuilder queryBuilder, int noOfResults);
	}
}
