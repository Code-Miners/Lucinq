using System.Collections.Generic;

namespace Lucinq.Core.Interfaces
{
	public interface IItemResult<T> : ISearchResult, IEnumerable<T>
	{
		List<T> Items { get; }
	}
}
