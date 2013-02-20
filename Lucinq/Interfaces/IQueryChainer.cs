using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
	public interface IQueryChainer
	{
		BooleanClause.Occur Occur { get; set; }

		BooleanClause.Occur ChildrenOccur { get; set; }

		IQueryBuilder Parent { get; }
	}
}
