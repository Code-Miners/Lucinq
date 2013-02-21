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
	public static class QueryBuilderExtensions
	{
		#region [ Setup Expressions ]

		public static IQueryBuilder Where(this IQueryBuilder inputQuery, Action<IQueryBuilder> inputExpression)
		{
			inputExpression(inputQuery);
			return inputQuery;
		}

		public static IQueryBuilder Setup(this IQueryBuilder inputQueryBuilder, params Action<IQueryBuilder>[] queries)
		{
			foreach (var item in queries)
			{
				item(inputQueryBuilder);
			}
			return inputQueryBuilder;
		}

		#endregion

		#region [ Term Expressions ]

		public static TermQuery Term(this IQueryBuilder inputQueryBuilder, string fieldName, string fieldValue, BooleanClause.Occur occur = null,  float? boost = null, string key = null)
		{
			Term term = new Term(fieldName, fieldValue);
			TermQuery query = new TermQuery(term);
			SetOccurValue(inputQueryBuilder, ref occur);
			SetBoostValue(query, boost);

			inputQueryBuilder.Add(query, occur, key);
			return query;
		}

		public static IQueryBuilder Terms(this IQueryBuilder inputQueryBuilder, string fieldName, string[] fieldValues, BooleanClause.Occur occur = null, float? boost = null)
		{
			var group = inputQueryBuilder.Group();
			foreach (var fieldValue in fieldValues)
			{
				group.Term(fieldName, fieldValue, occur, boost);
			}
			return group.Parent;
		}

		#endregion

		#region [ Fuzzy Expressions ]

		public static FuzzyQuery Fuzzy(this IQueryBuilder inputQueryBuilder, string fieldName, string fieldValue, BooleanClause.Occur occur = null, float? boost = null, string key = null)
		{
			Term term = new Term(fieldName, fieldValue);
			FuzzyQuery query = new FuzzyQuery(term);
			SetOccurValue(inputQueryBuilder, ref occur);
			SetBoostValue(query, boost);

			inputQueryBuilder.Add(query, occur, key);
			return query;
		}
		#endregion

		#region [ Phrase Expressions ]

		public static PhraseQuery Phrase(this IQueryBuilder inputQueryBuilder, int slop, float? boost = null, BooleanClause.Occur occur = null, string key = null)
		{
			PhraseQuery query = new PhraseQuery();
			SetOccurValue(inputQueryBuilder, ref occur);

			SetBoostValue(query, boost);
			query.SetSlop(slop);

			inputQueryBuilder.Add(query, occur, key);
			return query;
		}

		public static IQueryBuilder Phrase(this IQueryBuilder inputQueryBuilder, string fieldName, string[] fieldValues, int slop, BooleanClause.Occur occur = null, float? boost = null)
		{
			PhraseQuery phrase = inputQueryBuilder.Phrase(slop, boost, occur);
			foreach (var fieldValue in fieldValues)
			{
				phrase.AddTerm(fieldName, fieldValue);
			}
			return inputQueryBuilder;
		}

		#endregion

		#region [ Wildcard Expressions ]

		public static WildcardQuery WildCard(this IQueryBuilder inputQueryBuilder, string fieldName, string fieldValue, BooleanClause.Occur occur = null, float? boost = null, string key = null)
		{
			Term term = new Term(fieldName, fieldValue);
			WildcardQuery query = new WildcardQuery(term);
			SetOccurValue(inputQueryBuilder, ref occur);
			SetBoostValue(query, boost);

			inputQueryBuilder.Add(query, occur, key);
			return query;
		}

		public static IQueryBuilder WildCards(this IQueryBuilder inputQueryBuilder, string fieldName, string[] fieldValues, BooleanClause.Occur occur = null,
								  float? boost = null)
		{
			var group = inputQueryBuilder.Group();
			foreach (var fieldValue in fieldValues)
			{
				group.WildCard(fieldName, fieldValue, occur, boost);
			}
			return group.Parent;
		}
		#endregion

		#region [ Group Expressions ]

		public static IQueryBuilder And(this IQueryBuilder inputQueryBuilder, params Action<IQueryBuilder>[] queries)
		{
			return inputQueryBuilder.Group(BooleanClause.Occur.MUST, BooleanClause.Occur.MUST, queries).Parent;
		}

		public static IQueryBuilder And(this IQueryBuilder inputQueryBuilder, BooleanClause.Occur occur = null, params Action<IQueryBuilder>[] queries)
		{
			return inputQueryBuilder.Group(occur, BooleanClause.Occur.MUST, queries).Parent;
		}

		public static IQueryBuilder Or(this IQueryBuilder inputQueryBuilder, params Action<IQueryBuilder>[] queries)
		{
			return inputQueryBuilder.Group(BooleanClause.Occur.MUST, BooleanClause.Occur.SHOULD, queries).Parent;
		}

		public static IQueryBuilder Or(this IQueryBuilder inputQueryBuilder, BooleanClause.Occur occur = null, params Action<IQueryBuilder>[] queries)
		{
			return inputQueryBuilder.Group(occur, BooleanClause.Occur.SHOULD, queries).Parent;
		}

		public static IQueryBuilder Group(this IQueryBuilder inputQueryBuilder, BooleanClause.Occur occur = null, BooleanClause.Occur childrenOccur = null, params Action<IQueryBuilder>[] queries)
		{
			if (occur == null)
			{
				occur = BooleanClause.Occur.MUST;
			}
			var groupedBooleanQuery = inputQueryBuilder.AddGroup(occur, childrenOccur, queries);
			if (groupedBooleanQuery == null)
			{
				throw new Exception("An error occurred creating the inner query");
			}
			return groupedBooleanQuery;
		}

		private static IQueryBuilder AddGroup(this IQueryBuilder inputQueryBuilder, BooleanClause.Occur occur = null, BooleanClause.Occur childrenOccur = null, params Action<IQueryBuilder>[] queries)
		{
			if (occur == null)
			{
				occur = BooleanClause.Occur.MUST;
			}
			if (childrenOccur == null)
			{
				childrenOccur = BooleanClause.Occur.MUST;
			}

			IQueryBuilder queryBuilder = new QueryBuilder(inputQueryBuilder) { Occur = occur, ChildrenOccur = childrenOccur};
			foreach (var item in queries)
			{
				item(queryBuilder);
			}
			inputQueryBuilder.Groups.Add(queryBuilder);
			return queryBuilder;
		}

		#endregion

		#region [ Other Expressions ]

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

			occur = inputQueryBuilder != null ? inputQueryBuilder.ChildrenOccur : BooleanClause.Occur.MUST;
		}

		#endregion
	}
}
