using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
	public interface IQueryWithParent
	{
		BooleanQuery ParentQuery { get; }
	}
}
