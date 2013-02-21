using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearch<T> : ILuceneSearch
	{
		T Execute(Query query, int noOfResults);

		T Execute(IQueryBuilder queryBuilder, int noOfResults);
	}

	public interface ILuceneSearch
	{
		IndexSearcher IndexSearcher { get; }
	}
}
