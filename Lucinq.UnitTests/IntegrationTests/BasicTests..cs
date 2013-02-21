using System;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucinq.Interfaces;
using NUnit.Framework;

namespace Lucinq.UnitTests.IntegrationTests
{
	[TestFixture]
	public class BasicTests
	{
		#region [ Fields ]

		private LuceneSearch search;
		
		#endregion

		#region [ Properties ]

		public LuceneSearch Search
		{
			get { return search ?? (search = new LuceneSearch(GeneralConstants.Paths.BBCIndex)); }
		}

		#endregion

		[Test]
		public void SingleTermClauseSuccessful()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Term(BBCFields.Title, "africa");

			var results = ExecuteAndAssert(queryBuilder, 8);

			Assert.AreEqual(8, results.TotalHits);

			IQueryBuilder alternative = new QueryBuilder();
			alternative.Where(x => x.Term("_name", "work"));

			var results2 = Search.Execute(queryBuilder.Build(), 20);
			Assert.AreEqual(results.TotalHits, results2.TotalHits);
		}

		[Test]
		public void SingleTermSetupSuccessful()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup(x => x.Term(BBCFields.Title, "africa"));

			ExecuteAndAssert(queryBuilder, 8);
		}

		[Test]
		public void SimpleOrClauseSuccessful()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Or
				(
					x => x.Term(BBCFields.Title, "africa"),
					x => x.Term(BBCFields.Title, "europe")
				);

			ExecuteAndAssert(queryBuilder, 12);
		}

		[Test]
		public void SimpleAndClauseSuccessful()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.And
				(
					x => x.Term(BBCFields.Title, "africa"),
					x => x.Term(BBCFields.Title, "road")
				);

			ExecuteAndAssert(queryBuilder, 1);
		}

		[Test]
		public void RemoveAndReexecute()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Term(BBCFields.Title, "africa", key: "africacriteria");

			var results = ExecuteAndAssert(queryBuilder, 8);

			queryBuilder.Queries.Remove("africacriteria");
			queryBuilder.Term(BBCFields.Title, "report", key: "businesscriteria");

			Console.WriteLine("\r\nSecond Criteria");

			var results2 = ExecuteAndAssert(queryBuilder, 5);

			Assert.AreNotEqual(results.TotalHits, results2.TotalHits);
		}

		[Test]
		public void EasyOr()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Terms(BBCFields.Title, new[] {"europe", "africa"}, BooleanClause.Occur.SHOULD);
			ExecuteAndAssert(queryBuilder, 12);
		}

		/*[Test]
		public void SimpleNot()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Not().Term("_name", "home");
			var results = ExecuteAndAssert(queryBuilder, 12);
		}*/

		[Test]
		public void EasyAnd()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Terms(BBCFields.Title, new[] { "africa", "road" }, occur: BooleanClause.Occur.MUST);
			ExecuteAndAssert(queryBuilder, 1);
		}

		[Test]
		public void SimpleWildCardQuery()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup(x => x.WildCard(BBCFields.Description, "a*"));

			ExecuteAndAssert(queryBuilder, 902);
		}

		[Test]
		public void ChainedTerms()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "a*"),
					x => x.Term(BBCFields.Description, "police")
				);

			ExecuteAndAssert(queryBuilder, 17);
		}

		[Test]
		public void Group()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard("_name", "a*"),
					x => x.Term("_name", "work"),
					x => x.Group().Setup
							(
								y => y.Term("_name", "work")
							)
				);

			ExecuteAndAssert(queryBuilder, 4);

			throw new NotImplementedException("Needs finishing");
		}

		private TopDocs ExecuteAndAssert(IQueryBuilder queryBuilder, int numberOfHitsExpected)
		{
			TopDocs results = Search.Execute(queryBuilder.Build(), 20);

			foreach (Document document in Search.GetTopDocuments(results))
			{
				Console.WriteLine(document.GetValues(BBCFields.Title)[0]);
			}

			Assert.AreEqual(numberOfHitsExpected, results.TotalHits);
			
			return results;
		}
	}
}
