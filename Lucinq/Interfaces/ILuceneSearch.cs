using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
	public interface ILuceneSearch
	{
		IndexSearcher IndexSearcher { get; }
	}
}
