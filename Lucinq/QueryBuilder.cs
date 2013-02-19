using System.Collections.Generic;
using Lucene.Net.Search;
using Lucinq.Interfaces;

namespace Lucinq
{
	public class QueryBuilder : IQueryBuilder
	{
		#region [ Constructors ]

		public QueryBuilder()
		{
			Queries = new Dictionary<string, QueryReference>();
			Groups = new List<IQueryBuilder>();
			Occur = BooleanClause.Occur.MUST;
		}

		public QueryBuilder(IQueryBuilder parentQueryBuilder)
			: this()
		{
			Parent = parentQueryBuilder;
		}

		#endregion

		#region [ Properties ]

		public BooleanClause.Occur Occur { get; set; }

		public IQueryBuilder Parent { get; private set; }

		public Dictionary<string, QueryReference> Queries { get; private set; }

		public List<IQueryBuilder> Groups { get; private set; }

		#endregion

		#region [ Methods ]

		public void Add(Query query, BooleanClause.Occur occur, string key = null)
		{
			if (key == null)
			{
				key = "Query_" + Queries.Count;
			}
			QueryReference queryReference = new QueryReference{Occur = occur, Query = query};
			Queries.Add(key, queryReference);
		}

		public Query Build()
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
			return booleanQuery;
		}

		public IQueryBuilder End()
		{
			return Parent;
		}

		#endregion
	}

	public class QueryReference
	{
		public BooleanClause.Occur Occur { get; set; }

		public Query Query { get; set; }
	}
}
