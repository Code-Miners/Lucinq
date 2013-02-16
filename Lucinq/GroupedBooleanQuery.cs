using Lucene.Net.Search;
using Lucinq.Interfaces;

namespace Lucinq
{
	public class GroupedBooleanQuery : BooleanQuery, IQueryWithParent
	{
		public BooleanClause.Occur Occur { get; set; }

		public BooleanQuery ParentQuery { get; private set; }

		public GroupedBooleanQuery(BooleanQuery parentQuery)
		{
			ParentQuery = parentQuery;
		}

		public BooleanQuery End()
		{
			return ParentQuery;
		}
	}
}
