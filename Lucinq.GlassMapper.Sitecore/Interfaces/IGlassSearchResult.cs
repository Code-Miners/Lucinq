using Lucinq.Interfaces;

namespace Lucinq.GlassMapper.SitecoreIntegration.Interfaces
{
	public interface IGlassSearchResult : ISearchResult
	{
		ILuceneSearchResult LuceneSearchResult { get; }

		IGlassItemResult<T> GetPagedItems<T>(int start, int end, int multiplier = 3) where T : class;

		IGlassItemResult<T> GetTopItems<T>() where T : class;

		T GetItem<T>(int index = 0) where T : class;
	}
}
