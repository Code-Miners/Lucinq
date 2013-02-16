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

		public static BooleanQuery Where(this BooleanQuery inputQuery, Action<BooleanQuery> inputExpression)
		{
			inputExpression(inputQuery);
			return inputQuery;
		}

		public static TermQuery Term(this BooleanQuery inputQuery, string fieldName, string fieldValue, BooleanClause.Occur occur = null)
		{
			Term term = new Term(fieldName, fieldValue);
			BooleanQuery parentQuery = GetParentQuery(inputQuery);
			TermQuery query = new TermQuery(term);
			SetOccurValue(inputQuery, ref occur);
			parentQuery.Add(query, occur);
			return query;
		}

		public static PhraseQuery Phrase(this BooleanQuery inputQuery, string field, string text, BooleanClause.Occur occur = null)
		{
			BooleanQuery parentQuery = GetParentQuery(inputQuery);
			PhraseQuery query = new PhraseQuery();
			SetOccurValue(inputQuery, ref occur);
			query.AddTerm(field, text);
			parentQuery.Add(query, occur);
			return query;
		}

		public static PhraseQuery Phrase(this BooleanQuery inputQuery, BooleanClause.Occur occur = null)
		{
			BooleanQuery parentQuery = GetParentQuery(inputQuery);
			PhraseQuery query = new PhraseQuery();
			SetOccurValue(inputQuery, ref occur);
			parentQuery.Add(query, occur);
			return query;
		}

		public static WildcardQuery WildCard(this BooleanQuery inputQuery, string fieldName, string fieldValue, BooleanClause.Occur occur = null)
		{
			Term term = new Term(fieldName, fieldValue);
			BooleanQuery parentQuery = GetParentQuery(inputQuery);
			WildcardQuery query = new WildcardQuery(term);
			SetOccurValue(inputQuery, ref occur);
			parentQuery.Add(query, occur);
			return query;
		}

		public static BooleanQuery And(this BooleanQuery inputQuery, params Action<GroupedBooleanQuery>[] queries)
		{
			return inputQuery.Group(BooleanClause.Occur.MUST, queries);
		}

		public static BooleanQuery Or(this BooleanQuery inputQuery, params Action<GroupedBooleanQuery>[] queries)
		{
			var groupedBooleanQuery = inputQuery.AddGroup(BooleanClause.Occur.SHOULD, queries);
			if (groupedBooleanQuery == null)
			{
				throw new Exception("An error occurred creating the inner query");
			}
			return groupedBooleanQuery.ParentQuery;
		}

		public static BooleanQuery Group(this BooleanQuery inputQuery, BooleanClause.Occur occur = null, params Action<GroupedBooleanQuery>[] queries)
		{
			var groupedBooleanQuery = inputQuery.AddGroup(BooleanClause.Occur.SHOULD, queries);
			if (groupedBooleanQuery == null)
			{
				throw new Exception("An error occurred creating the inner query");
			}
			return inputQuery.AddGroup(BooleanClause.Occur.SHOULD, queries);
		}

		private static GroupedBooleanQuery AddGroup(this BooleanQuery inputQuery, BooleanClause.Occur occur = null, params Action<GroupedBooleanQuery>[] queries)
		{
			GroupedBooleanQuery query = new GroupedBooleanQuery(inputQuery) { Occur = BooleanClause.Occur.MUST };
			foreach (var item in queries)
			{
				item(query);
			}
			inputQuery.Add(query, BooleanClause.Occur.MUST);
			return query;
		}

		public static BooleanQuery Setup(this BooleanQuery inputQuery, params Action<BooleanQuery>[] queries)
		{
			foreach (var item in queries)
			{
				item(inputQuery);
			}
			return inputQuery;
		}

		public static Query Raw(this BooleanQuery booleanQuery, string field, string queryText, BooleanClause.Occur occur = null)
		{
			Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
			QueryParser queryParser = new QueryParser(Version.LUCENE_29, field, analyzer);
			Query query = queryParser.Parse(queryText);
			booleanQuery.Add(query, occur);
			return query;
		}

		#endregion

		#region [ Helper Methods ]

		private static void SetOccurValue(BooleanQuery inputQuery, ref BooleanClause.Occur occur)
		{
			if (occur != null)
			{
				return;
			}

			GroupedBooleanQuery parentQuery = inputQuery as GroupedBooleanQuery;
			occur = parentQuery != null ? parentQuery.Occur : BooleanClause.Occur.MUST;
		}

		private static BooleanQuery GetParentQuery(Query inputQuery)
		{
			BooleanQuery parentQuery;
			if (inputQuery is BooleanQuery)
			{
				parentQuery = inputQuery as BooleanQuery;
			}
			else if (inputQuery is IQueryWithParent)
			{
				IQueryWithParent wrappedQuery = inputQuery as IQueryWithParent;
				parentQuery = wrappedQuery.ParentQuery;
			}
			else
			{
				throw new ArgumentException("Wrong parameter type - should be IWrappedQuery or BooleanQuery", "inputQuery");
			}
			return parentQuery;
		}

		#endregion
	}
}
