using Lucene.Net.Search;
using Lucinq.Enums;

namespace Lucinq.Querying
{
	public class QueryReference
	{
		public Matches Occur { get; set; }

		public Query Query { get; set; }
	}
}
