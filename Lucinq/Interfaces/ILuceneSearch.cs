namespace Lucinq.Interfaces
{
	public interface ILuceneSearch<T> : ILuceneSearchExecutor<T>, ILuceneSearcherAccessor where T : class, ISearchResult
	{

	}
}
