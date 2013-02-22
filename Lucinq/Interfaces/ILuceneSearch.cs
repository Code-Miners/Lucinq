using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearch<T> : ILuceneSearch
	{
		T Execute(Query query, int noOfResults, Sort sort = null);

		T Execute(IQueryBuilder queryBuilder, int noOfResults, Sort sort = null);
	}

	public interface ILuceneSearch
	{
		IndexSearcher IndexSearcher { get; }
	}
}
