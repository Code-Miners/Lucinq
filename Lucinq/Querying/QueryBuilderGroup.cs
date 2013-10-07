using System;
using Lucene.Net.Search;
using Lucinq.Enums;
using Lucinq.Extensions;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
    public partial class QueryBuilder : IQueryBuilder
    {
        #region [ Build Methods ]

        /// <summary>
        /// Adds a query to the current group
        /// </summary>
        /// <param name="query">The query to add</param>
        /// <param name="occur">The occur value for the query</param>
        /// <param name="key">A key to allow manipulation from the dictionary later on (a default key will be generated if none is specified</param>
        public virtual void Add(Query query, Matches occur, string key = null)
        {
            if (key == null)
            {
                key = "Query_" + Queries.Count;
            }
            SetOccurValue(this, ref occur);
            QueryReference queryReference = new QueryReference { Occur = occur, Query = query };
            Queries.Add(key, queryReference);
        }

		  private void Add(Filter filter)
		  {
			  CurrentFilter = filter;
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
                booleanQuery.Add(query.Query, GetLuceneOccur(query.Occur));
            }

            foreach (IQueryBuilder query in Groups)
            {
                booleanQuery.Add(query.Build(), GetLuceneOccur(query.Occur));
            }

            BuildSort();

            return booleanQuery;
        }

        public virtual BooleanClause.Occur GetLuceneOccur(Matches matches)
        {
            return matches.GetLuceneOccurance();
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

        #region [ Group Expressions ]

        /// <summary>
        /// Creates a simple group that MUST occur, each item of which MUST occur by default
        /// </summary>
        /// <param name="queries">The lamdba expressions showing queries</param>
        /// <returns></returns>
        public virtual IQueryBuilder And(params Action<IQueryBuilder>[] queries)
        {
            Group(Matches.Always, Matches.Always, queries);
            return this;
        }

        /// <summary>
        /// Creates a simple group allowing the specification of whether it should occur, each item of which MUST occur by default
        /// </summary>
        /// <param name="occur">Whether the group must / should occur</param>
        /// <param name="queries">The lamdba expressions showing queries</param>
        public virtual IQueryBuilder And(Matches occur = Matches.NotSet, params Action<IQueryBuilder>[] queries)
        {
            Group(occur, Matches.Always, queries);
            return this;
        }

        /// <summary>
        /// Creates a simple group that MUST occur, each item of which SHOULD occur by default
        /// </summary>
        /// <param name="queries">The lamdba expressions showing queries</param>
        public virtual IQueryBuilder Or(params Action<IQueryBuilder>[] queries)
        {
            return Group(Matches.Always, Matches.Sometimes, queries).Parent;
        }

        /// <summary>
        /// Creates a simple group allowing the specification of whether it should occur, each item of which SHOULD occur by default
        /// </summary>
        /// <param name="occur">Whether the group must / should occur</param>
        /// <param name="queries">The lamdba expressions showing queries</param>
        public virtual IQueryBuilder Or(Matches occur = Matches.NotSet, params Action<IQueryBuilder>[] queries)
        {
            return Group(occur, Matches.Sometimes, queries).Parent;
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

        protected virtual IQueryBuilder CreateNewChildGroup(Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet)
        {
            return new QueryBuilder(this) { Occur = occur, DefaultChildrenOccur = childrenOccur };
        }

        #endregion
    }
}
