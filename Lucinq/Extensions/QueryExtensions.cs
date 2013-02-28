using Lucene.Net.Index;
using Lucene.Net.Search;

namespace Lucinq.Extensions
{
	public static class QueryExtensions
	{
		public static PhraseQuery AddTerm(this PhraseQuery query, string field, string value, bool? caseSensitive = null)
		{
			if (caseSensitive.HasValue && !caseSensitive.Value || !caseSensitive.HasValue)
			{
				value = value.ToLowerInvariant();
			}
			query.Add(new Term(field, value));
			return query;
		}
	}
}
