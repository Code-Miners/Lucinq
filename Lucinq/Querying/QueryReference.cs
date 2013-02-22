using Lucene.Net.Search;

namespace Lucinq.Querying
{
	public class QueryReference
	{
		public BooleanClause.Occur Occur { get; set; }

		public Query Query { get; set; }
	}
}
