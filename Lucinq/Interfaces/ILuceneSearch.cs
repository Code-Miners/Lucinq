using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearch<T> : ILuceneSearchExecutor<T>, ILuceneSearcherAccessor where T : class, ISearchResult
	{

	}
}
