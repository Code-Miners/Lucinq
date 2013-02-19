using System;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucinq.Interfaces;
using Version = Lucene.Net.Util.Version;

namespace Lucinq
{
	public static class BooleanQueryExtensions
	{
		#region [ Boolean Extensions ]

		public static IQueryBuilder Where(this IQueryBuilder inputQuery, Action<IQueryBuilder> inputExpression)
		{
			inputExpression(inputQuery);
			return inputQuery;
		}

		public static TermQuery Term(this IQueryBuilder inputQueryBuilder, string fieldName, string fieldValue, float? boost = null, BooleanClause.Occur occur = null, string key = null)
		{
			Term term = new Term(fieldName, fieldValue);
			TermQuery query = new TermQuery(term);
			SetOccurValue(inputQueryBuilder, ref occur);
			SetBoostValue(query, boost);

			inputQueryBuilder.Add(query, occur, key);
			return query;
		}

		public static PhraseQuery Phrase(this IQueryBuilder inputQueryBuilder, string field, string text, float? boost = null, BooleanClause.Occur occur = null, string key = null)
		{
			PhraseQuery query = new PhraseQuery();
			SetOccurValue(inputQueryBuilder, ref occur);

			if (!String.IsNullOrEmpty(field))
			{
				query.AddTerm(field, text);
			}

			SetBoostValue(query, boost);

			inputQueryBuilder.Add(query, occur, key);
			return query;
		}

		public static PhraseQuery Phrase(this IQueryBuilder inputQueryBuilder, float? boost = null, BooleanClause.Occur occur = null, string key = null)
		{
			return inputQueryBuilder.Phrase(null, null, boost, occur, key);
		}

		public static WildcardQuery WildCard(this IQueryBuilder inputQueryBuilder, string fieldName, string fieldValue, float? boost = null, BooleanClause.Occur occur = null, string key = null)
		{
			Term term = new Term(fieldName, fieldValue);
			WildcardQuery query = new WildcardQuery(term);
			SetOccurValue(inputQueryBuilder, ref occur);
			SetBoostValue(query, boost);

			inputQueryBuilder.Add(query, occur, key);
			return query;
		}

		public static IQueryBuilder And(this IQueryBuilder inputQueryBuilder, params Action<QueryBuilder>[] queries)
		{
			return inputQueryBuilder.Group(BooleanClause.Occur.MUST, queries).Parent;
		}

		public static IQueryBuilder Or(this IQueryBuilder inputQueryBuilder, params Action<QueryBuilder>[] queries)
		{
			return inputQueryBuilder.Group(BooleanClause.Occur.SHOULD, queries).Parent;
		}

		public static IQueryBuilder Group(this IQueryBuilder inputQueryBuilder, BooleanClause.Occur occur = null, params Action<QueryBuilder>[] queries)
		{
			var groupedBooleanQuery = inputQueryBuilder.AddGroup(occur, queries);
			if (groupedBooleanQuery == null)
			{
				throw new Exception("An error occurred creating the inner query");
			}
			return groupedBooleanQuery;
		}

		private static IQueryBuilder AddGroup(this IQueryBuilder inputQueryBuilder, BooleanClause.Occur occur = null, params Action<QueryBuilder>[] queries)
		{
			if (occur == null)
			{
				occur = BooleanClause.Occur.MUST;
			}
			QueryBuilder queryBuilder = new QueryBuilder(inputQueryBuilder) { Occur = occur };
			foreach (var item in queries)
			{
				item(queryBuilder);
			}
			inputQueryBuilder.Groups.Add(queryBuilder);
			return queryBuilder;
		}

		public static IQueryBuilder Setup(this IQueryBuilder inputQueryBuilder, params Action<IQueryBuilder>[] queries)
		{
			foreach (var item in queries)
			{
				item(inputQueryBuilder);
			}
			return inputQueryBuilder;
		}

		public static Query Raw(this QueryBuilder booleanQueryBuilder, string field, string queryText, BooleanClause.Occur occur = null, string key = null)
		{
			Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
			QueryParser queryParser = new QueryParser(Version.LUCENE_29, field, analyzer);
			Query query = queryParser.Parse(queryText);
			booleanQueryBuilder.Add(query, occur, key);
			return query;
		}

		#endregion

		#region [ Helper Methods ]

		private static void SetBoostValue(Query query, float? boost)
		{
			if (!boost.HasValue)
			{
				return;
			}
			query.SetBoost(boost.Value);
		}

		private static void SetOccurValue(IQueryBuilder inputQueryBuilder, ref BooleanClause.Occur occur)
		{
			if (occur != null)
			{
				return;
			}

			occur = inputQueryBuilder != null ? inputQueryBuilder.Occur : BooleanClause.Occur.MUST;
		}

		#endregion
	}
}
