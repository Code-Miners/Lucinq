using Lucene.Net.Index;
using Lucene.Net.Search;

namespace Lucinq.Extensions
{
	public static class QueryExtensions
	{
		public static PhraseQuery AddTerm(this PhraseQuery query, string field, string text)
		{
			query.Add(new Term(field, text));
			return query;
		}
	}
}
