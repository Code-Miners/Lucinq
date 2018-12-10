using System;
using System.Collections.Generic;
using System.Linq;
using Lucinq.Core.Enums;
using Lucinq.Core.Interfaces;

namespace Lucinq.Core.Querying
{
	public partial class QueryBuilder
	{
		#region [ Fields ]

        private Matches defaultChildrenOccur;
	    private Func<LucinqGroupQuery> groupQueryFunc;

		#endregion

		#region [ Constructors ]

		public QueryBuilder() : this(() => new LucinqQueryRoot {Matches = Matches.Sometimes})
		{
		}

	    private void Initialise(Func<LucinqGroupQuery> queryFunc)
	    {
	        this.groupQueryFunc = queryFunc;
	        Queries = new Dictionary<string, LucinqQuery>();
	        Groups = new List<IQueryBuilder>();
	        Occur = Matches.Always;
	        SortFields = new List<LucinqSortField>();
	    }

	    public QueryBuilder(Func<LucinqGroupQuery> queryFunc)
		{
		    Initialise(queryFunc);
        }

        public QueryBuilder(params Action<IQueryBuilder>[] queries)
            : this()
        {
            AddQueries(queries);
        }

		#endregion

		#region [ Properties ]

		/// <summary>
		/// Gets or sets the occurance value for the query builder
		/// </summary>
		public Matches Occur { get; set; }

		/// <summary>
		/// Gets or sets whether the query is to be case sensitive
		/// </summary>
		public bool CaseSensitive { get; set; }

		/// <summary>
		/// Gets or sets the default occur value for child queries within the builder
		/// </summary>
		public Matches DefaultChildrenOccur
		{
			get
			{
				return GetDefaultChildrenOccur();
			}
			set { defaultChildrenOccur = value; }
		}

		/// <summary>
		/// Gets the child queries in the builder
		/// </summary>
		public Dictionary<string, LucinqQuery> Queries { get; private set; }

		/// <summary>
		/// Gets the child groups in the builder
		/// </summary>
		public List<IQueryBuilder> Groups { get; private set; }

		public List<LucinqSortField> SortFields { get; private set; }

		public LucinqFilter CurrentFilter { get; private set; }

		#endregion

		#region [ Setup Expressions ]

		/// <summary>
		/// A setup method to aid multiple query setup
		/// </summary>
		/// <param name="queries">Comma seperated lambda actions</param>
		/// <returns>The input querybuilder</returns>
		public virtual IQueryBuilder Setup(params Action<IQueryBuilder>[] queries)
		{
			AddQueries(queries);
			return this;
		}

	    protected void AddQueries(params Action<IQueryBuilder>[] queries)
	    {
            foreach (Action<IQueryBuilder> item in queries)
            {
                item(this);
            }
	    }

		#endregion

		#region [ Term Expressions ]

		 public virtual void PrefixedWith(String fieldname, String value, Matches occur = Matches.NotSet, float? boost = null, String key = null)
		{
		    LucinqPrefixQuery query = new LucinqPrefixQuery(new LucinqTerm(fieldname, value));
		    query.Matches = occur;
			SetBoostValue(query, boost);

			Add(query, key);
		}

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
        public virtual void Term(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null)
		{
			LucinqTerm term = GetTerm(fieldName, fieldValue, caseSensitive);
		    LucinqTermQuery query = new LucinqTermQuery(term)
		    {
		        Matches = occur
		    };

		    SetBoostValue(query, boost);

			Add(query, key);
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
        public virtual IQueryBuilder Terms(string fieldName, string[] fieldValues, Matches occur = Matches.NotSet, float? boost = null, bool? caseSensitive = null)
		{
			var group = Group(childrenOccur:occur);
			foreach (var fieldValue in fieldValues)
			{
				group.Term(fieldName, fieldValue, occur, boost, caseSensitive:caseSensitive);
			}
			return this;
		}

		#endregion

		#region [ Keywords ]

        public virtual void Keyword(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null, string key = null,
		                     bool? caseSensitive = null)
		{
		    LucinqTerm term = GetTerm(fieldName, fieldValue, caseSensitive);
		    LucinqKeywordQuery query = new LucinqKeywordQuery(term)
		    {
                Matches = occur
		    };

            SetBoostValue(query, boost);
		    Add(query, key);
		}

        public virtual IQueryBuilder Keywords(string fieldName, string[] fieldValues, Matches occur = Matches.NotSet, float? boost = null, string key = null,
		                      bool? caseSensitive = null)
		{
			var group = Group();
			foreach (var fieldValue in fieldValues)
			{
				group.Keyword(fieldName, fieldValue, occur, boost, key, caseSensitive);
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
        public virtual void Fuzzy(string fieldName, string fieldValue, float minSimilarity, Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null)
		{
			LucinqTerm term = GetTerm(fieldName, fieldValue, caseSensitive);
		    LucinqFuzzyQuery query = new LucinqFuzzyQuery(term, minSimilarity)
		    {
                Matches = occur
		    };
			SetBoostValue(query, boost);

			Add(query, key);
		}

		#endregion

		#region [ Phrase Expressions ]

	    /// <summary>
	    /// Sets up and adds a phrase query object allowing the search for an explcit term in the field
	    /// To add terms, use the AddTerm() query extension
	    /// </summary>
	    /// <param name="occur">Whether it must, must not or should occur in the field</param>
	    /// <param name="slop">The allowed distance between the terms</param>
	    /// <param name="fieldValues">The fields to use</param>
	    /// <param name="boost">A boost multiplier (1 is default / normal).</param>
	    /// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
	    /// <returns>The generated phrase query object</returns>
	    public virtual void Phrase(int slop, KeyValuePair<string, string>[] fieldValues, float? boost = null, Matches occur = Matches.NotSet, string key = null, bool? caseSensitive = null)
		{
			LucinqPhraseQuery query = new LucinqPhraseQuery(fieldValues.Select(x => GetTerm(x.Key, x.Value, caseSensitive)).ToArray())
			{
                Matches = occur
			};

			SetBoostValue(query, boost);
			query.Slop = slop;

			Add(query, key);
		}

		#endregion

		#region [ Range Expressions ]

		public virtual void TermRange(string fieldName, string rangeStart, string rangeEnd, bool includeLower = true, bool includeUpper = true,
                                        Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null)
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

		    LucinqRangeQuery<string> rangeQuery = new LucinqRangeQuery<string>(fieldName, rangeStart, rangeEnd, includeLower, includeUpper, LucinqConstants.AdapterKeys.StringRangeQuery)
		    {
                Matches = occur
		    };
			SetBoostValue(rangeQuery, boost);
			Add(rangeQuery, key);
		}

        public virtual void NumericRange(string fieldName, int minValue, int maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                                    int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null)
		{
		    LucinqRangeQuery<int> rangeQuery = new LucinqRangeQuery<int>(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax, LucinqConstants.AdapterKeys.IntRangeQuery)
		    {
                Matches = occur
		    };
			SetBoostValue(rangeQuery, boost);
			Add(rangeQuery, key);
		}

        public virtual void NumericRange(string fieldName, float minValue, float maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                                    int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null)
		{
		    LucinqRangeQuery<float> rangeQuery = new LucinqRangeQuery<float>(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax, LucinqConstants.AdapterKeys.FloatRangeQuery)
		    {
                Matches = occur
		    };
			SetBoostValue(rangeQuery, boost);
			Add(rangeQuery, key);
		}

        public virtual void NumericRange(string fieldName, double minValue, double maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                            int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null)
		{
		    LucinqRangeQuery<double> rangeQuery = new LucinqRangeQuery<double>(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax, LucinqConstants.AdapterKeys.DoubleRangeQuery)
		    {
                Matches = occur
		    };
			SetBoostValue(rangeQuery, boost);
			Add(rangeQuery, key);
		}

        public virtual void NumericRange(string fieldName, long minValue, long maxValue, Matches occur = Matches.NotSet, float? boost = null,
									int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null)
		{
		    LucinqRangeQuery<long> rangeQuery = new LucinqRangeQuery<long>(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax, LucinqConstants.AdapterKeys.LongRangeQuery)
		    {
                Matches = occur
		    };
			SetBoostValue(rangeQuery, boost);
			Add(rangeQuery, key);
		}

		 public virtual void DateRange(string fieldName, DateTime minValue, DateTime maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                    int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null)
		{
            LucinqRangeQuery<float> rangeQuery = new LucinqRangeQuery<float>(fieldName, precisionStep, minValue.Ticks, maxValue.Ticks, includeMin, includeMax, LucinqConstants.AdapterKeys.FloatRangeQuery)
            {
                Matches = occur
            };
			SetBoostValue(rangeQuery, boost);
			Add(rangeQuery, key);
		}

		public virtual void Filter(LucinqFilter filter)
		{
			Add(filter);
		}

		

		#endregion

		#region [ Sort Expressions ]


        /// <summary>
        /// A convenience helper for sorting to make it more readable.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="sortDescending"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
	    public virtual IQueryBuilder Sort(string fieldName, bool sortDescending = false, SortType sortType = SortType.String)
	    {
	        return Sort(fieldName, sortDescending, (int) sortType);
	    }

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
				sortType = (int)SortType.String;
			}

			LucinqSortField sortField = new LucinqSortField(fieldName, sortType.Value, sortDescending);
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
        public virtual void WildCard(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null)
		{
			LucinqTerm term = GetTerm(fieldName, fieldValue, caseSensitive);
			LucinqWildcardQuery query = new LucinqWildcardQuery(term)
			{
                Matches = occur
			};
			SetBoostValue(query, boost);

			Add(query, key);
		}

        public virtual IQueryBuilder WildCards(string fieldName, string[] fieldValues, Matches occur = Matches.NotSet,
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

		#region [ Other Expressions ]

        /*public virtual LucinqQuery Raw(string field, string queryText, Matches occur = Matches.NotSet, float? boost = null, string key = null, Analyzer analyzer = null)
		{
			if (analyzer == null)
			{
                analyzer = new StandardAnalyzer(Version.LUCENE_30);
			}
			QueryParser queryParser = new QueryParser(Version.LUCENE_30, field, analyzer);
			LucinqQuery query = queryParser.Parse(queryText);
			SetBoostValue(query, boost);
			Add(query, occur, key);
			return query;
		}*/

		#endregion

		#region [ Helper Methods ]

		protected virtual LucinqTerm GetTerm(string field, string value, bool? caseSensitive = null)
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
			return new LucinqTerm(field, value);
		}

		protected virtual void SetBoostValue(LucinqQuery query, float? boost)
		{
			if (!boost.HasValue)
			{
				return;
			}

			query.Boost = boost.Value;
		}

		protected virtual void SetOccurValue(IQueryBuilder inputQueryBuilder, ref Matches occur)
		{
            if (occur != Matches.NotSet)
			{
				return;
			}

			occur = inputQueryBuilder != null ? inputQueryBuilder.DefaultChildrenOccur : Matches.Always;
		}

		/// <summary>
		/// Gets the default child occur value
		/// </summary>
		/// <returns></returns>
		protected virtual Matches GetDefaultChildrenOccur()
		{
		    if (defaultChildrenOccur == Matches.NotSet)
		    {
		        defaultChildrenOccur = Matches.Always;
		    }
		    return defaultChildrenOccur;
		}

		#endregion
	}
}
