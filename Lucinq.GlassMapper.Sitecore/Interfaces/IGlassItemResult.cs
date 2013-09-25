using System.Collections.Generic;
using Lucinq.Interfaces;

namespace Lucinq.GlassMapper.SitecoreIntegration.Interfaces
{
	public interface IGlassItemResult<T> : ISearchResult, IEnumerable<T>
	{
		List<T> Items { get; }
	}
}
