using System;
using Lucene.Net.Analysis;
using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
	public interface IQueryBuilder : IHeirarchicalQueryGroup
	{
		#region [ Methods ]

		/// <summary>
		/// Adds a query to the current group
		/// </summary>
		/// <param name="query">The query to add</param>
		/// <param name="occur">The occur value for the query</param>
		/// <param name="key">A key to allow manipulation from the dictionary later on (a default key will be generated if none is specified</param>
		void Add(Query query, BooleanClause.Occur occur, string key = null);

		/// <summary>
		/// Builds the query
		/// </summary>
		/// <returns>The query built from the queries and groups that have been added</returns>
		Query Build();

		/// <summary>
		/// Ends the current query set returning to the parent query builder
		/// </summary>
		/// <returns></returns>
		IQueryBuilder End();

		/// <summary>
		/// A simple single item setup method
		/// Usage .Where(x => x.Term("field", "value));
		/// </summary>
		/// <param name="inputExpression">The lambda expression to be executed</param>
		/// <returns>The input querybuilder</returns>
		IQueryBuilder Where(Action<IQueryBuilder> inputExpression);

		/// <summary>
		/// A setup method to aid multiple query setup
		/// </summary>
		/// <param name="queries">Comma seperated lambda actions</param>
		/// <returns>The input querybuilder</returns>
		IQueryBuilder Setup(params Action<IQueryBuilder>[] queries);

		/// <summary>
		/// Sets up and adds a term query object allowing the search for an explcit term in the field
		/// Note: Wildcards should use the wildcard query type.
		/// </summary>
		/// <param name="fieldName">The field name to search within</param>
		/// <param name="fieldValue">The value to match</param>
		/// <param name="occur">Whether it must, must not or should occur in the field</param>
		/// <param name="boost">A boost multiplier (1 is default / normal).</param>
		/// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
		/// <param name="caseSensitive"></param>
		/// <returns>The generated term query</returns>
		TermQuery Term(string fieldName, string fieldValue, BooleanClause.Occur occur = null,  float? boost = null, string key = null, bool? caseSensitive = null);

		/// <summary>
		/// Sets up term queries for each of the values specified
		/// (usually or / and / not in depending on the occur specified)
		/// </summary>
		/// <param name="fieldName">The field name to search within</param>
		/// <param name="fieldValues">The values to match</param>
		/// <param name="occur">Whether it must, must not or should occur in the field</param>
		/// <param name="boost">A boost multiplier (1 is default / normal).</param>
		/// <param name="caseSensitive">Whether the value is explicitly case sensitive (else use the query builders value)</param>
		/// <returns>The input query builder</returns>
		IQueryBuilder Terms(string fieldName, string[] fieldValues, BooleanClause.Occur occur = null, float? boost = null, bool? caseSensitive = null);

		/// <summary>
		/// Sets up and adds a fuzzy query object allowing the search for an explcit term in the field
		/// </summary>
		/// <param name="fieldName">The field name to search within</param>
		/// <param name="fieldValue">The value to match</param>
		/// <param name="occur">Whether it must, must not or should occur in the field</param>
		/// <param name="boost">A boost multiplier (1 is default / normal).</param>
		/// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
		/// <param name="caseSensitive">Whether the value is explicitly case sensitive (else use the query builders value)</param>
		/// <returns>The generated fuzzy query object</returns>
		FuzzyQuery Fuzzy(string fieldName, string fieldValue, BooleanClause.Occur occur = null, float? boost = null, string key = null, bool? caseSensitive = null);

		/// <summary>
		/// Sets up and adds a phrase query object allowing the search for an explcit term in the field
		/// To add terms, use the AddTerm() query extension
		/// </summary>
		/// <param name="occur">Whether it must, must not or should occur in the field</param>
		/// <param name="slop">The allowed distance between the terms</param>
		/// <param name="boost">A boost multiplier (1 is default / normal).</param>
		/// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
		/// <returns>The generated phrase query object</returns>
		PhraseQuery Phrase(int slop, float? boost = null, BooleanClause.Occur occur = null, string key = null);

		IQueryBuilder Phrase(string fieldName, string[] fieldValues, int slop, BooleanClause.Occur occur = null, float? boost = null, bool? caseSensitive = null);

		/// <summary>
		/// Sets up and adds a wildcard query object allowing the search for an explcit term in the field
		/// </summary>
		/// <param name="fieldName">The field name to search within</param>
		/// <param name="fieldValue">The value to match</param>
		/// <param name="occur">Whether it must, must not or should occur in the field</param>
		/// <param name="boost">A boost multiplier (1 is default / normal).</param>
		/// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
		/// <param name="caseSensitive">Whether the value is explicitly case sensitive (else use the query builders value)</param>
		/// <returns>The generated wildcard query object</returns>
		WildcardQuery WildCard(string fieldName, string fieldValue, BooleanClause.Occur occur = null, float? boost = null, string key = null, bool? caseSensitive = null);

		IQueryBuilder WildCards(string fieldName, string[] fieldValues, BooleanClause.Occur occur = null,
		                                        float? boost = null, bool? caseSensitive = null);

		/// <summary>
		/// Creates a simple group that MUST occur, each item of which MUST occur by default
		/// </summary>
		/// <param name="queries">The lamdba expressions showing queries</param>
		/// <returns></returns>
		IQueryBuilder And(params Action<IQueryBuilder>[] queries);

		/// <summary>
		/// Creates a simple group allowing the specification of whether it should occur, each item of which MUST occur by default
		/// </summary>
		/// <param name="occur">Whether the group must / should occur</param>
		/// <param name="queries">The lamdba expressions showing queries</param>
		IQueryBuilder And(BooleanClause.Occur occur = null, params Action<IQueryBuilder>[] queries);

		/// <summary>
		/// Creates a simple group that MUST occur, each item of which SHOULD occur by default
		/// </summary>
		/// <param name="queries">The lamdba expressions showing queries</param>
		IQueryBuilder Or(params Action<IQueryBuilder>[] queries);

		/// <summary>
		/// Creates a simple group allowing the specification of whether it should occur, each item of which SHOULD occur by default
		/// </summary>
		/// <param name="occur">Whether the group must / should occur</param>
		/// <param name="queries">The lamdba expressions showing queries</param>
		IQueryBuilder Or(BooleanClause.Occur occur = null, params Action<IQueryBuilder>[] queries);

		/// <summary>
		/// Creates a simple group allowing the specification of whether it should occur, and specification of each items occurance.
		/// </summary>
		/// <param name="occur">Whether the group must / should occur</param>
		/// <param name="childrenOccur">Whether the child query should occur by default</param>
		/// <param name="queries">The lamdba expressions showing queries</param>
		IQueryBuilder Group(BooleanClause.Occur occur = null, BooleanClause.Occur childrenOccur = null, params Action<IQueryBuilder>[] queries);

		Query Raw(string field, string queryText, BooleanClause.Occur occur = null, float? boost = null, string key = null, Analyzer analyzer = null);

		/// <summary>
		/// Querys values to return results within the specified range of terms
		/// </summary>
		/// <param name="fieldName">The field name</param>
		/// <param name="rangeStart">The start of the range to search</param>
		/// <param name="rangeEnd">The end of the range to search</param>
		/// <param name="includeLower">A boolean denoting whether to include the lowest value</param>
		/// <param name="includeUpper"></param>
		/// <param name="occur"></param>
		/// <param name="boost"></param>
		/// <param name="key"></param>
		/// <param name="caseSensitive">Whether the value is explicitly case sensitive (else use the query builders value)</param>
		/// <returns></returns>
		TermRangeQuery TermRange(string fieldName, string rangeStart, string rangeEnd, bool includeLower = true,
		                                        bool includeUpper = true,
		                                        BooleanClause.Occur occur = null, float? boost = null, string key = null, bool? caseSensitive = null);

		IQueryBuilder Sort(string fieldName, int? sortType = null);

		#endregion
	}
}