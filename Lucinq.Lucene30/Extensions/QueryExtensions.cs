using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucinq.Core.Interfaces;

namespace Lucinq.Lucene30.Extensions
{
	public static class QueryExtensions
	{
		/// <summary>
		/// Adds a term to an existing phrase query
		/// </summary>
		/// <param name="query">The phrase query to add the term to</param>
		/// <param name="field">The field</param>
		/// <param name="value">The value</param>
		/// <param name="caseSensitive">A boolean denoting whether or not the field should retain case</param>
		/// <returns>The input phrase query object</returns>
		public static PhraseQuery AddTerm(this PhraseQuery query, string field, string value, bool caseSensitive = false)
		{
			if (!caseSensitive)
			{
				value = value.ToLowerInvariant();
			}
			query.Add(new Term(field, value));
			return query;
		}

		/// <summary>
		/// Adds a phrase term taking into consideration the query builder it is being added to
		/// </summary>
		/// <param name="query">The phrase query to add the term to</param>
		/// <param name="queryBuilder">The parent query builder</param>
		/// <param name="field">The field to be added to the query</param>
		/// <param name="value">The value for the field</param>
		/// <param name="caseSensitive">A boolean denoting whether to retain case</param>
		/// <returns></returns>
		public static PhraseQuery AddTerm(this PhraseQuery query, IQueryBuilder queryBuilder, string field, string value,
		                                  bool? caseSensitive = null)
		{
			return query.AddTerm(field, value, caseSensitive.HasValue ? caseSensitive.Value : queryBuilder.CaseSensitive);
		}
	}
}
