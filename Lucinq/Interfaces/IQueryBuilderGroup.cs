using System;
using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Lucinq.Enums;

namespace Lucinq.Interfaces
{
    public interface IQueryBuilderGroup
	{
		#region [ Properties ]

		bool CaseSensitive { get; set; }

		KeywordAnalyzer KeywordAnalyzer { get; }

		#endregion

		#region [ Methods ]

		/// <summary>
		/// Adds a query to the current group
		/// </summary>
		/// <param name="query">The query to add</param>
		/// <param name="occur">The occur value for the query</param>
		/// <param name="key">A key to allow manipulation from the dictionary later on (a default key will be generated if none is specified</param>
        void Add(Query query, Matches occur, string key = null);

		/// <summary>
		/// Builds the query
		/// </summary>
		/// <returns>The query built from the queries and groups that have been added</returns>
		Query Build();

		/// <summary>
		/// A setup method to aid multiple query setup
		/// </summary>
		/// <param name="queries">Comma seperated lambda actions</param>
		/// <returns>The input querybuilder</returns>
        IQueryBuilder Setup(params Action<IQueryBuilder>[] queries);
        
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
        IQueryBuilder Terms(string fieldName, string[] fieldValues, Matches occur = Matches.NotSet, float? boost = null, bool? caseSensitive = null);

        /// <summary>
        /// Creates a set of keywords
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValues"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>s
        /// <param name="caseSensitive"></param>
        /// <returns></returns>
        IQueryBuilder Keywords(string fieldName, string[] fieldValues, Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null);

        IQueryBuilder WildCards(string fieldName, string[] fieldValues, Matches occur = Matches.NotSet,
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
        IQueryBuilder And(Matches occur = Matches.NotSet, params Action<IQueryBuilder>[] queries);

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
        IQueryBuilder Or(Matches occur = Matches.NotSet, params Action<IQueryBuilder>[] queries);

		/// <summary>
		/// Creates a simple group allowing the specification of whether it should occur, and specification of each items occurance.
		/// </summary>
		/// <param name="occur">Whether the group must / should occur</param>
		/// <param name="childrenOccur">Whether the child query should occur by default</param>
		/// <param name="queries">The lamdba expressions showing queries</param>
        IQueryBuilder Group(Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, params Action<IQueryBuilder>[] queries);

		/// <summary>
		/// Creates a raw query lucene query
		/// </summary>
		/// <param name="field"></param>
		/// <param name="queryText"></param>
		/// <param name="occur"></param>
		/// <param name="boost"></param>
		/// <param name="key"></param>
		/// <param name="analyzer"></param>
		/// <returns></returns>
        Query Raw(string field, string queryText, Matches occur = Matches.NotSet, float? boost = null, string key = null, Analyzer analyzer = null);

		IQueryBuilder Sort(string fieldName, bool sortDescending = false, int? sortType = null);

        IQueryBuilder Phrase(string fieldName, string[] fieldValues, int slop, Matches occur = Matches.NotSet, float? boost = null, bool? caseSensitive = null);

		#endregion
	}
}