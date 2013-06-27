using System;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearch<T> : ILuceneSearchExecutor<T>, IDisposable, ILuceneSearcherAccessor where T : class, ISearchResult
	{

	}
}
