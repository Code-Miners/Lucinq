using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearcherAccessor
	{
		IndexSearcher IndexSearcher { get; }
	}
}
