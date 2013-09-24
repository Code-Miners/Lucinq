using System.Collections.Generic;
using Lucinq.Interfaces;

namespace Lucinq.GlassMapper.SitecoreIntegration.Interfaces
{
	public interface IGlassItemResult<T> : ISearchResult
	{
		List<T> Items { get; }
	}
}
