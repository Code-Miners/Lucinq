using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucinq.Extensions;
using Lucinq.Interfaces;
using Version = Lucene.Net.Util.Version;

namespace Lucinq.Querying
{
	public class QueryBuilder : IQueryBuilder
	{
		#region [ Fields ]

		private BooleanClause.Occur defaultChildrenOccur;

		private KeywordAnalyzer keywordAnalyzer;

		#endregion

		#region [ Constructors ]

		public QueryBuilder()
		{
			Queries = new Dictionary<string, QueryReference>();
			Groups = new List<IQueryBuilder>();
			Occur = BooleanClause.Occur.MUST;
			SortFields = new List<SortField>();
		}

		public QueryBuilder(IQueryBuilder parentQueryBuilder)
			: this()
		{
			Parent = parentQueryBuilder;
		}

		#endregion

		#region [ Properties ]

		/// <summary>
		/// Gets or sets the occurance value for the query builder
		/// </summary>
		public BooleanClause.Occur Occur { get; set; }

		/// <summary>
		/// Gets or sets whether the query is to be case sensitive
		/// </summary>
		public bool CaseSensitive { get; set; }

		/// <summary>
		/// Gets or sets the default occur value for child queries within the builder
		/// </summary>
		public BooleanClause.Occur DefaultChildrenOccur
		{
			get
			{
				return GetDefaultChildrenOccur();
			}
			set { defaultChildrenOccur = value; }
		}

		/// <summary>
		/// Gets the parent query builder
		/// </summary>
		public IQueryBuilder Parent { get; private set; }

		/// <summary>
		/// Gets the child queries in the builder
		/// </summary>
		public Dictionary<string, QueryReference> Queries { get; private set; }

		/// <summary>
		/// Gets the child groups in the builder
		/// </summary>
		public List<IQueryBuilder> Groups { get; private set; }

		public List<SortField> SortFields { get; private set; }

		public Sort CurrentSort { get; set; }

		/// <summary>
		/// Gets the keyword analyzer used by the keyword queries
		/// </summary>
		public KeywordAnalyzer KeywordAnalyzer { get { return keywordAnalyzer ?? (keywordAnalyzer = new KeywordAnalyzer()); } }

		#endregion

		#region [ Build Methods ]

		/// <summary>
		/// Adds a query to the current group
		/// </summary>
		/// <param name="query">The query to add</param>
		/// <param name="occur">The occur value for the query</param>
		/// <param name="key">A key to allow manipulation from the dictionary later on (a default key will be generated if none is specified</param>
		public virtual void Add(Query query, BooleanClause.Occur occur, string key = null)
		{
			if (key == null)
			{
				key = "Query_" + Queries.Count;
			}
			SetOccurValue(this, ref occur);
			QueryReference queryReference = new QueryReference { Occur = occur, Query = query };
			Queries.Add(key, queryReference);
		}

		/// <summary>
		/// Builds the query
		/// </summary>
		/// <returns>The query built from the queries and groups that have been added</returns>
		public virtual Query Build()
		{
			BooleanQuery booleanQuery = new BooleanQuery();
			foreach (QueryReference query in Queries.Values)
			{
				booleanQuery.Add(query.Query, query.Occur);
			}

			foreach (IQueryBuilder query in Groups)
			{
				booleanQuery.Add(query.Build(), query.Occur);
			}

			BuildSort();

			return booleanQuery;
		}

		/// <summary>
		/// Ends the current query set returning to the parent query builder
		/// </summary>
		/// <returns></returns>
		public virtual IQueryBuilder End()
		{
			return Parent;
		}

		public virtual void BuildSort()
		{
			if (SortFields.Count == 0)
			{
				return;
			}
			CurrentSort = new Sort(SortFields.ToArray());
		}

		#endregion

		#region [ Setup Expressions ]

		/// <summary>
		/// A simple single item setup method
		/// Usage .Where(x => x.Term("field", "value));
		/// </summary>
		/// <param name="inputExpression">The lambda expression to be executed</param>
		/// <returns>The input querybuilder</returns>
		public virtual IQueryBuilder Where(Action<IQueryBuilder> inputExpression)
		{
			inputExpression(this);
			return this;
		}

		/// <summary>
		/// A setup method to aid multiple query setup
		/// </summary>
		/// <param name="queries">Comma seperated lambda actions</param>
		/// <returns>The input querybuilder</returns>
		public virtual IQueryBuilder Setup(params Action<IQueryBuilder>[] queries)
		{
			foreach (Action<IQueryBuilder> item in queries)
			{
				item(this);
			}
			return this;
		}

		#endregion

		#region [ Term Expressions ]

		/// <summary>
		/// Sets up and adds a term query object allowing the search for an explcit term in the field
		/// Note: Wildcards should use the wildcard query type.
		/// </summary>
		/// <param name="fieldName">The field name to search within</param>
		/// <param name="fieldValue">The value to match</param>
		/// <param name="occur">Whether it must, must not or should occur in the field</param>
		/// <param name="boost">A boost multiplier (1 is default / normal).</param>
		/// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
		/// <param name="caseSensitive">A boolean denoting whether or not to retain case</param>
		/// <returns>The generated term query</returns>
		public virtual TermQuery Term(string fieldName, string fieldValue, BooleanClause.Occur occur = null, float? boost = null, string key = null, bool? caseSensitive = null)
		{
			Term term = GetTerm(fieldName, fieldValue, caseSensitive);
			TermQuery query = new TermQuery(term);
			SetBoostValue(query, boost);

			Add(query, occur, key);
			return query;
		}

		/// <summary>
		/// Sets up term queries for each of the values specified
		/// (usually or / and / not in depending on the occur specified)
		/// </summary>
		/// <param name="fieldName">The field name to search within</param>
		/// <param name="fieldValues">The values to match</param>
		/// <param name="occur">Whether it must, must not or should occur in the field</param>
		/// <param name="boost">A boost multiplier (1 is default / normal).</param>
		/// <param name="caseSensitive">A boolean denoting whether or not to retain case</param>
		/// <returns>The input query builder</returns>
		public virtual IQueryBuilder Terms(string fieldName, string[] fieldValues, BooleanClause.Occur occur = null, float? boost = null, bool? caseSensitive = null)
		{
			var group = Group();
			foreach (var fieldValue in fieldValues)
			{
				group.Term(fieldName, fieldValue, occur, boost, caseSensitive:caseSensitive);
			}
			return this;
		}

		#endregion

		#region [ Keywords ]

		public virtual Query Keyword(string fieldName, string fieldValue, BooleanClause.Occur occur = null, float? boost = null, string key = null,
		                     bool? caseSensitive = null)
		{
			if (!caseSensitive.HasValue || !caseSensitive.Value)
			{
				fieldValue = fieldValue.ToLower();
			}
			return Raw(fieldName, fieldValue, occur, boost, key, KeywordAnalyzer);
		}

		public virtual IQueryBuilder Keywords(string fieldName, string[] fieldValues, BooleanClause.Occur occur = null, float? boost = null, string key = null,
		                      bool? caseSensitive = null)
		{
			var group = Group();
			foreach (var fieldValue in fieldValues)
			{
				group.Raw(fieldName, fieldValue, occur, boost, key, KeywordAnalyzer);
			}
			return this;
		}

		#endregion

		#region [ Fuzzy Expressions ]

		/// <summary>
		/// Sets up and adds a fuzzy query object allowing the search for an explcit term in the field
		/// </summary>
		/// <param name="fieldName">The field name to search within</param>
		/// <param name="fieldValue">The value to match</param>
		/// <param name="occur">Whether it must, must not or should occur in the field</param>
		/// <param name="boost">A boost multiplier (1 is default / normal).</param>
		/// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
		/// <param name="caseSensitive">A boolean denoting whether or not to retain case</param>
		/// <returns>The generated fuzzy query object</returns>
		public virtual FuzzyQuery Fuzzy(string fieldName, string fieldValue, BooleanClause.Occur occur = null, float? boost = null, string key = null, bool? caseSensitive = null)
		{
			Term term = GetTerm(fieldName, fieldValue, caseSensitive);
			FuzzyQuery query = new FuzzyQuery(term);
			SetBoostValue(query, boost);

			Add(query, occur, key);
			return query;
		}

		#endregion

		#region [ Phrase Expressions ]

		/// <summary>
		/// Sets up and adds a phrase query object allowing the search for an explcit term in the field
		/// To add terms, use the AddTerm() query extension
		/// </summary>
		/// <param name="occur">Whether it must, must not or should occur in the field</param>
		/// <param name="slop">The allowed distance between the terms</param>
		/// <param name="boost">A boost multiplier (1 is default / normal).</param>
		/// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
		/// <returns>The generated phrase query object</returns>
		public virtual PhraseQuery Phrase(int slop, float? boost = null, BooleanClause.Occur occur = null, string key = null)
		{
			PhraseQuery query = new PhraseQuery();

			SetBoostValue(query, boost);
			query.SetSlop(slop);

			Add(query, occur, key);
			return query;
		}

		/// <summary>
		/// Adds a phrase query with a number of pre-specified values
		/// </summary>
		/// <param name="fieldName">The field name to query</param>
		/// <param name="fieldValues">The array of field values</param>
		/// <param name="slop">The distance between values</param>
		/// <param name="occur">The occurance for the query</param>
		/// <param name="boost">The boost value for the query</param>
		/// <param name="caseSensitive">A boolean denoting whether or not to retain case</param>
		/// <returns>The input query builder</returns>
		public virtual IQueryBuilder Phrase(string fieldName, string[] fieldValues, int slop, BooleanClause.Occur occur = null, float? boost = null, bool? caseSensitive = null)
		{
			PhraseQuery phrase = Phrase(slop, boost, occur);
			foreach (var fieldValue in fieldValues)
			{
				phrase.AddTerm(this, fieldName, fieldValue, caseSensitive);
			}
			return this;
		}

		#endregion

		#region [ Range Expressions ]

		public virtual TermRangeQuery TermRange(string fieldName, string rangeStart, string rangeEnd, bool includeLower = true, bool includeUpper = true,
										BooleanClause.Occur occur = null, float? boost = null, string key = null, bool? caseSensitive = null)
		{
			if (caseSensitive.HasValue)
			{
				if (!caseSensitive.Value)
				{
					rangeStart = rangeStart.ToLowerInvariant();
					rangeEnd = rangeEnd.ToLowerInvariant();
				}
			}
			else if(!CaseSensitive)
			{
				rangeStart = rangeStart.ToLowerInvariant();
				rangeEnd = rangeEnd.ToLowerInvariant();
			}
			TermRangeQuery query = new TermRangeQuery(QueryParser.Escape(fieldName), rangeStart, rangeEnd, includeLower, includeUpper);
			SetBoostValue(query, boost);
			Add(query, occur, key);
			return query;
		}

		public virtual NumericRangeQuery NumericRange(string fieldName, int minValue, int maxValue, BooleanClause.Occur occur = null, float? boost = null, 
													int precisionStep = 1, bool includeMin = true, bool includeMax = true, string key = null)
		{
			NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewIntRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
			SetBoostValue(numericRangeQuery, boost);
			Add(numericRangeQuery, occur, key);
			return numericRangeQuery;
		}

		public virtual NumericRangeQuery NumericRange(string fieldName, float minValue, float maxValue, BooleanClause.Occur occur = null, float? boost = null,
													int precisionStep = 1, bool includeMin = true, bool includeMax = true, string key = null)
		{
			NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewFloatRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
			SetBoostValue(numericRangeQuery, boost);
			Add(numericRangeQuery, occur, key);
			return numericRangeQuery;
		}

		public virtual NumericRangeQuery NumericRange(string fieldName, double minValue, double maxValue, BooleanClause.Occur occur = null, float? boost = null,
											int precisionStep = 1, bool includeMin = true, bool includeMax = true, string key = null)
		{
			NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewDoubleRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
			SetBoostValue(numericRangeQuery, boost);
			Add(numericRangeQuery, occur, key);
			return numericRangeQuery;
		}

		public virtual NumericRangeQuery NumericRange(string fieldName, long minValue, long maxValue, BooleanClause.Occur occur = null, float? boost = null,
									int precisionStep = 1, bool includeMin = true, bool includeMax = true, string key = null)
		{
			NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewLongRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
			SetBoostValue(numericRangeQuery, boost);
			Add(numericRangeQuery, occur, key);
			return numericRangeQuery;
		}

		public virtual NumericRangeQuery DateRange(string fieldName, DateTime minValue, DateTime maxValue, BooleanClause.Occur occur = null, float? boost = null,
									int precisionStep = 1, bool includeMin = true, bool includeMax = true, string key = null)
		{
			NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewLongRange(fieldName, precisionStep, minValue.Ticks, maxValue.Ticks, includeMin, includeMax);
			SetBoostValue(numericRangeQuery, boost);
			Add(numericRangeQuery, occur, key);
			return numericRangeQuery;
		}



		#endregion

		#region [ Sort Expressions ]

		/// <summary>
		/// Sorts the results by the corresponding field
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="sortDescending"></param>
		/// <param name="sortType"></param>
		/// <returns></returns>
		public virtual IQueryBuilder Sort(string fieldName, bool sortDescending = false, int? sortType = null)
		{
			if (!sortType.HasValue)
			{
				sortType = SortField.STRING;
			}

			SortField sortField = new SortField(fieldName, sortType.Value, sortDescending);
			SortFields.Add(sortField);
			return this;
		}

		#endregion

		#region [ Wildcard Expressions ]

		/// <summary>
		/// Sets up and adds a wildcard query object allowing the search for an explcit term in the field
		/// </summary>
		/// <param name="fieldName">The field name to search within</param>
		/// <param name="fieldValue">The value to match</param>
		/// <param name="occur">Whether it must, must not or should occur in the field</param>
		/// <param name="boost">A boost multiplier (1 is default / normal).</param>
		/// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
		/// <param name="caseSensitive"></param>
		/// <returns>The generated wildcard query object</returns>
		public virtual WildcardQuery WildCard(string fieldName, string fieldValue, BooleanClause.Occur occur = null, float? boost = null, string key = null, bool? caseSensitive = null)
		{
			Term term = GetTerm(fieldName, fieldValue, caseSensitive);
			WildcardQuery query = new WildcardQuery(term);
			SetBoostValue(query, boost);

			Add(query, occur, key);
			return query;
		}

		public virtual IQueryBuilder WildCards(string fieldName, string[] fieldValues, BooleanClause.Occur occur = null,
								  float? boost = null, bool? caseSensitive = null)
		{
			var group = Group();
			foreach (var fieldValue in fieldValues)
			{
				group.WildCard(fieldName, fieldValue, occur, boost, caseSensitive:caseSensitive);
			}
			return this;
		}
		#endregion

		#region [ Group Expressions ]

		/// <summary>
		/// Creates a simple group that MUST occur, each item of which MUST occur by default
		/// </summary>
		/// <param name="queries">The lamdba expressions showing queries</param>
		/// <returns></returns>
		public virtual IQueryBuilder And(params Action<IQueryBuilder>[] queries)
		{
			Group(BooleanClause.Occur.MUST, BooleanClause.Occur.MUST, queries);
			return this;
		}

		/// <summary>
		/// Creates a simple group allowing the specification of whether it should occur, each item of which MUST occur by default
		/// </summary>
		/// <param name="occur">Whether the group must / should occur</param>
		/// <param name="queries">The lamdba expressions showing queries</param>
		public virtual IQueryBuilder And(BooleanClause.Occur occur = null, params Action<IQueryBuilder>[] queries)
		{
			Group(occur, BooleanClause.Occur.MUST, queries);
			return this;
		}

		/// <summary>
		/// Creates a simple group that MUST occur, each item of which SHOULD occur by default
		/// </summary>
		/// <param name="queries">The lamdba expressions showing queries</param>
		public virtual IQueryBuilder Or(params Action<IQueryBuilder>[] queries)
		{
			return Group(BooleanClause.Occur.MUST, BooleanClause.Occur.SHOULD, queries).Parent;
		}

		/// <summary>
		/// Creates a simple group allowing the specification of whether it should occur, each item of which SHOULD occur by default
		/// </summary>
		/// <param name="occur">Whether the group must / should occur</param>
		/// <param name="queries">The lamdba expressions showing queries</param>
		public virtual IQueryBuilder Or(BooleanClause.Occur occur = null, params Action<IQueryBuilder>[] queries)
		{
			return Group(occur, BooleanClause.Occur.SHOULD, queries).Parent;
		}

		/// <summary>
		/// Creates a simple group allowing the specification of whether it should occur, and specification of each items occurance.
		/// </summary>
		/// <param name="occur">Whether the group must / should occur</param>
		/// <param name="childrenOccur">Whether the child query should occur by default</param>
		/// <param name="queries">The lamdba expressions showing queries</param>
		public virtual IQueryBuilder Group(BooleanClause.Occur occur = null, BooleanClause.Occur childrenOccur = null, params Action<IQueryBuilder>[] queries)
		{
			if (occur == null)
			{
				occur = BooleanClause.Occur.MUST;
			}
			var groupedBooleanQuery = AddChildGroup(occur, childrenOccur, queries);
			if (groupedBooleanQuery == null)
			{
				throw new Exception("An error occurred creating the inner query");
			}
			return groupedBooleanQuery;
		}

		#endregion

		#region [ Other Expressions ]

		public virtual Query Raw(string field, string queryText, BooleanClause.Occur occur = null, float? boost = null,  string key = null, Analyzer analyzer = null)
		{
			if (analyzer == null)
			{
				analyzer = new StandardAnalyzer(Version.LUCENE_29);
			}
			QueryParser queryParser = new QueryParser(Version.LUCENE_29, field, analyzer);
			Query query = queryParser.Parse(queryText);
			SetBoostValue(query, boost);
			Add(query, occur, key);
			return query;
		}

		#endregion

		#region [ Helper Methods ]

		protected virtual Term GetTerm(string field, string value, bool? caseSensitive = null)
		{
			if (caseSensitive.HasValue)
			{
				if (!caseSensitive.Value)
				{
					value = value.ToLowerInvariant();
				}
			}
			else if (!CaseSensitive)
			{
				value = value.ToLowerInvariant();
			}
			return new Term(field, value);
		}

		protected virtual IQueryBuilder AddChildGroup(BooleanClause.Occur occur = null, BooleanClause.Occur childrenOccur = null, params Action<IQueryBuilder>[] queries)
		{
			if (occur == null)
			{
				occur = BooleanClause.Occur.MUST;
			}
			if (childrenOccur == null)
			{
				childrenOccur = BooleanClause.Occur.MUST;
			}

			IQueryBuilder queryBuilder = CreateNewChildGroup(occur, childrenOccur);
			foreach (var item in queries)
			{
				item(queryBuilder);
			}
			Groups.Add(queryBuilder);
			return queryBuilder;
		}

		protected virtual IQueryBuilder CreateNewChildGroup(BooleanClause.Occur occur = null, BooleanClause.Occur childrenOccur = null)
		{
			return new QueryBuilder(this) {Occur = occur, DefaultChildrenOccur = childrenOccur};
		}

		protected virtual void SetBoostValue(Query query, float? boost)
		{
			if (!boost.HasValue)
			{
				return;
			}
			query.SetBoost(boost.Value);
		}

		protected virtual void SetOccurValue(IQueryBuilder inputQueryBuilder, ref BooleanClause.Occur occur)
		{
			if (occur != null)
			{
				return;
			}

			occur = inputQueryBuilder != null ? inputQueryBuilder.DefaultChildrenOccur : BooleanClause.Occur.MUST;
		}

		/// <summary>
		/// Gets the default child occur value
		/// </summary>
		/// <returns></returns>
		protected virtual BooleanClause.Occur GetDefaultChildrenOccur()
		{
			return defaultChildrenOccur ?? (defaultChildrenOccur = BooleanClause.Occur.MUST);
		}

		#endregion

		public override string ToString()
		{
			return String.Join(Environment.NewLine, Queries.Values.Select(x => x.Query.ToString()));
		}
	}
}
