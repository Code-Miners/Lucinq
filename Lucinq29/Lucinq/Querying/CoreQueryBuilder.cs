using System;
using System.Collections.Generic;
using Lucinq.Enums;
using Lucinq.Interfaces;
using Lucinq.Providers;

namespace Lucinq.Querying
{
    public abstract class CoreQueryBuilder : IQueryBuilder
    {
        private Matches defaultChildrenOccur;

        protected abstract IQueryProviderService QueryProviderService { get; }

        protected CoreQueryBuilder()
        {
            Queries = new Dictionary<string, IQueryReference>();
            Groups = new List<IQueryBuilder>();
            Occur = Matches.Always;
        }

        protected CoreQueryBuilder(IQueryBuilder parentQueryBuilder)
			: this()
		{
			Parent = parentQueryBuilder;
		}

        /// <summary>
        /// Gets the child queries in the builder
        /// </summary>
        public Dictionary<string, IQueryReference> Queries { get; private set; }

        /// <summary>
        /// Gets the child groups in the builder
        /// </summary>
        public List<IQueryBuilder> Groups { get; private set; }

        public ISortProvider CurrentSort { get; private set; }

        public IFilterProvider CurrentFilter { get; private set; }

        /// <summary>
        /// Gets the parent query builder
        /// </summary>
        public IQueryBuilder Parent { get; private set; }

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

        public IPhraseQueryProvider Phrase(int slop, float? boost, Matches occur, string key)
        {
            IPhraseQueryProvider provider = this.QueryProviderService.GetPhraseQueryProvider(slop);
            if (boost.HasValue)
            {
                provider.Boost = boost.Value;
            }

            provider.Slop = slop;

            Add(provider, occur, key);
            return provider;
        }

        /// <summary>
        /// Adds a query to the current group
        /// </summary>
        /// <param name="query">The query to add</param>
        /// <param name="occur">The occur value for the query</param>
        /// <param name="key">A key to allow manipulation from the dictionary later on (a default key will be generated if none is specified</param>
        public virtual void Add(IQueryProvider query, Matches occur, string key = null)
        {
            if (key == null)
            {
                key = "Query_" + Queries.Count;
            }
            SetOccurValue(this, ref occur);
            IQueryReference queryReference = new QueryReference { Occur = occur, QueryProvider = query };
            Queries.Add(key, queryReference);
        }

        /// <summary>
        /// Builds the query
        /// </summary>
        /// <returns>The query built from the queries and groups that have been added</returns>
        public virtual IBooleanQueryProvider Build()
        {
            IBooleanQueryProvider booleanQueryProvider = this.QueryProviderService.GetBooleanQueryProvider();
            foreach (IQueryReference query in Queries.Values)
            {
                booleanQueryProvider.Add(query.QueryProvider, query.Occur);
            }

            foreach (IQueryBuilder query in Groups)
            {
                booleanQueryProvider.Add(query.Build(), query.Occur);
            }

            BuildSort();

            return booleanQueryProvider;
        }

        public abstract void BuildSort();

        public virtual IQueryProvider Keyword(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null, string key = null,
                     bool? caseSensitive = null)
        {
            if (!caseSensitive.HasValue || !caseSensitive.Value)
            {
                fieldValue = fieldValue.ToLower();
            }

            return Raw(fieldName, fieldValue, occur, boost, key);
        }

        IQueryProvider IQueryBuilderIndividual.Fuzzy(
            string fieldName,
            string fieldValue,
            Matches occur,
            float? boost,
            string key,
            bool? caseSensitive)
        {
            return this.Fuzzy(fieldName, fieldValue, occur, boost, key, caseSensitive);
        }

        protected virtual IQueryBuilder AddChildGroup(Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, params Action<IQueryBuilder>[] queries)
        {
            if (occur == Matches.NotSet)
            {
                occur = Matches.Always;
            }
            if (childrenOccur == Matches.NotSet)
            {
                childrenOccur = Matches.Always;
            }

            IQueryBuilder queryBuilder = CreateNewChildGroup(occur, childrenOccur);
            foreach (var item in queries)
            {
                item(queryBuilder);
            }
            Groups.Add(queryBuilder);
            return queryBuilder;
        }

        /// <summary>
        /// Creates a simple group allowing the specification of whether it should occur, and specification of each items occurance.
        /// </summary>
        /// <param name="occur">Whether the group must / should occur</param>
        /// <param name="childrenOccur">Whether the child query should occur by default</param>
        /// <param name="queries">The lamdba expressions showing queries</param>
        public virtual IQueryBuilder Group(Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, params Action<IQueryBuilder>[] queries)
        {
            if (occur == Matches.NotSet)
            {
                occur = Matches.Always;
            }
            var groupedBooleanQuery = AddChildGroup(occur, childrenOccur, queries);
            if (groupedBooleanQuery == null)
            {
                throw new Exception("An error occurred creating the inner query");
            }
            return groupedBooleanQuery;
        }

        IQueryProvider IQueryBuilderGroup.Raw(
            string field,
            string queryText,
            Matches occur,
            float? boost,
            string key)
        {
            return this.Raw(field, queryText, occur, boost, key);
        }

        public virtual IQueryBuilder CreateOrGroup(Matches occur, params Action<IQueryBuilder>[] queries)
        {
            return Group(occur, Matches.Sometimes, queries);
        }

        /// <summary>
        /// Creates a new instance of an or group that MUST occur
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        public virtual IQueryBuilder CreateOrGroup(params Action<IQueryBuilder>[] queries)
        {
            return CreateOrGroup(Matches.Always, queries);
        }

        /// <summary>
        /// Gets a new instance of an and group
        /// </summary>
        /// <param name="occur">Whether the group should occur</param>
        /// <param name="queries">The queries</param>
        /// <returns></returns>
        public virtual IQueryBuilder CreateAndGroup(Matches occur, params Action<IQueryBuilder>[] queries)
        {
            return Group(occur, Matches.Always, queries);
        }

        /// <summary>
        /// Creates a simple group that MUST occur, each item of which MUST occur by default
        /// </summary>
        /// <param name="queries">The lamdba expressions showing queries</param>
        /// <returns>The original query builder</returns>
        public virtual IQueryBuilder And(params Action<IQueryBuilder>[] queries)
        {
            return And(Matches.Always, queries);
        }

        /// <summary>
        /// Creates a simple group allowing the specification of whether it should occur, each item of which MUST occur by default
        /// </summary>
        /// <param name="occur">Whether the group must / should occur</param>
        /// <param name="queries">The lamdba expressions showing queries</param>
        public virtual IQueryBuilder And(Matches occur, params Action<IQueryBuilder>[] queries)
        {
            CreateAndGroup(occur, queries);
            return this;
        }

        /// <summary>
        /// Creates a simple group that MUST occur, each item of which SHOULD occur by default
        /// </summary>
        /// <param name="queries">The lamdba expressions showing queries</param>
        public virtual IQueryBuilder Or(params Action<IQueryBuilder>[] queries)
        {
            return Or(Matches.Always, queries);
        }

        /// <summary>
        /// Creates a simple group allowing the specification of whether it should occur, each item of which SHOULD occur by default
        /// </summary>
        /// <param name="occur">Whether the group must / should occur</param>
        /// <param name="queries">The lamdba expressions showing queries</param>
        public virtual IQueryBuilder Or(Matches occur, params Action<IQueryBuilder>[] queries)
        {
            CreateOrGroup(occur, queries);
            return this;
        }

        /// <summary>
        /// A convenience helper for sorting to make it more readable.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="sortDescending"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
        public virtual IQueryBuilder Sort(string fieldName, bool sortDescending = false, SortType sortType = SortType.String)
        {
            return Sort(fieldName, sortDescending, (int)sortType);
        }

        public abstract IQueryBuilder Sort(string fieldName, bool sortDescending = false, int? sortType = null);

        public virtual IQueryBuilder Phrase(string fieldName, string[] fieldValues, int slop,
            Matches occur = Matches.NotSet, float? boost = null, bool? caseSensitive = null, string key = null)
        {
            var provider = this.Phrase(slop, boost, occur, key);
            foreach (var fieldValue in fieldValues)
            {
                provider.AddTerm(fieldName, fieldValue, caseSensitive);
            }
            return this;
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

        /// <summary>
        /// Creates a new instance of an and group.
        /// </summary>
        /// <returns>The new instance</returns>
        public virtual IQueryBuilder CreateAndGroup(params Action<IQueryBuilder>[] queries)
        {
            return CreateAndGroup(Matches.Always, queries);
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

        protected abstract IQueryBuilder CreateNewChildGroup(Matches occur = Matches.NotSet,
            Matches childrenOccur = Matches.NotSet);

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
            var group = Group();
            foreach (var fieldValue in fieldValues)
            {
                group.Term(fieldName, fieldValue, occur, boost, caseSensitive: caseSensitive);
            }
            return this;
        }

        public abstract INumericRangeQueryProvider<long> DateRange(
            string fieldName,
            DateTime minValue,
            DateTime maxValue,
            Matches occur = Matches.NotSet,
            float? boost = null,
            int precisionStep = Int32.MaxValue,
            bool includeMin = true,
            bool includeMax = true,
            string key = null);

        public virtual IQueryBuilder WildCards(string fieldName, string[] fieldValues, Matches occur = Matches.NotSet,
                          float? boost = null, bool? caseSensitive = null)
        {
            var group = Group();
            foreach (var fieldValue in fieldValues)
            {
                group.WildCard(fieldName, fieldValue, occur, boost, caseSensitive: caseSensitive);
            }
            return this;
        }

        public abstract ITermRangeQueryProvider TermRange(
            string fieldName,
            string rangeStart,
            string rangeEnd,
            bool includeLower = true,
            bool includeUpper = true,
            Matches occur = Matches.NotSet,
            float? boost = null,
            string key = null,
            bool? caseSensitive = null);

        public abstract INumericRangeQueryProvider<int> NumericRange(
            string fieldName,
            int minValue,
            int maxValue,
            Matches occur = Matches.NotSet,
            float? boost = null,
            int precisionStep = Int32.MaxValue,
            bool includeMin = true,
            bool includeMax = true,
            string key = null);

        public abstract INumericRangeQueryProvider<float> NumericRange(
            string fieldName,
            float minValue,
            float maxValue,
            Matches occur = Matches.NotSet,
            float? boost = null,
            int precisionStep = Int32.MaxValue,
            bool includeMin = true,
            bool includeMax = true,
            string key = null);

        public abstract INumericRangeQueryProvider<double> NumericRange(
            string fieldName,
            double minValue,
            double maxValue,
            Matches occur = Matches.NotSet,
            float? boost = null,
            int precisionStep = Int32.MaxValue,
            bool includeMin = true,
            bool includeMax = true,
            string key = null);

        public abstract INumericRangeQueryProvider<long> NumericRange(
            string fieldName,
            long minValue,
            long maxValue,
            Matches occur = Matches.NotSet,
            float? boost = null,
            int precisionStep = Int32.MaxValue,
            bool includeMin = true,
            bool includeMax = true,
            string key = null);

        public virtual IPrefixQueryProvider PrefixedWith(String fieldname, String value, Matches occur = Matches.NotSet, float? boost = null, String key = null)
        {
            IPrefixQueryProvider provider = this.QueryProviderService.GetPrefixQueryProvider(fieldname, value);
            QueryProviderService.SetBoost(provider, boost);

            Add(provider, occur, key);

            return provider;
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
        public virtual IQueryProvider Term(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null)
        {
            var provider = this.QueryProviderService.GetTermQueryProvider(fieldName, fieldValue, caseSensitive);

            QueryProviderService.SetBoost(provider, boost);

            Add(provider, occur, key);
            return provider;
        }

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
        public virtual IFuzzyQueryProvider Fuzzy(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null)
        {
            IFuzzyQueryProvider fuzzyQueryProvider = this.QueryProviderService.GetFuzzyQueryProvider(fieldName, fieldValue);
            return fuzzyQueryProvider;
        }

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
        public virtual IWildcardQueryProvider WildCard(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null)
        {
            IWildcardQueryProvider provider = this.QueryProviderService.GetWildcardQueryProvider();
            provider.Field = fieldName;
            provider.Value = fieldValue;
            if (caseSensitive.HasValue)
            {
                provider.CaseSensitive = caseSensitive.Value;
            }

            QueryProviderService.SetBoost(provider, boost);

            return provider;
        }

        public virtual IRawQueryProvider Raw(string field, string queryText, Matches occur = Matches.NotSet, float? boost = null, string key = null)
        {
            IRawQueryProvider provider = this.QueryProviderService.GetRawQueryProvider(field, queryText);
            QueryProviderService.SetBoost(provider, boost);

            return provider;
        }

    }
}
