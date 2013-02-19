using System;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucinq.Interfaces;
using NUnit.Framework;

namespace Lucinq.UnitTests
{
	[TestFixture]
	public class FacetSearchTests
	{
		#region [ Fields ]

		private const string indexPath = @"D:\tfs\3chillies.visualstudio.com\TwoBirds\Data\indexes\BirdAndBird";

		private LuceneSearch search;
		
		#endregion

		#region [ Properties ]

		public LuceneSearch Search
		{
			get { return search ?? (search = new LuceneSearch(indexPath)); }
		}

		#endregion

		[Test]
		public void SingleTermClauseSuccessful()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Term("_name", "work");

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
			queryBuilder.Setup(x => x.Term("_name", "work"));

			ExecuteAndAssert(queryBuilder, 8);
		}

		[Test]
		public void SimpleOrClauseSuccessful()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Or
				(
					x => x.Term("_name", "work"),
					x => x.Term("_name", "text")
				);

			ExecuteAndAssert(queryBuilder, 16);
		}

		[Test]
		public void SimpleAndClauseSuccessful()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.And
				(
					x => x.Term("_name", "work"),
					x => x.Term("_name", "highlights")
				);

			ExecuteAndAssert(queryBuilder, 4);
		}

		[Test]
		public void RemoveAndReexecute()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Term("_name", "home", key: "highlightscriteria");

			var results = ExecuteAndAssert(queryBuilder, 4);

			queryBuilder.Queries.Remove("highlightscriteria");
			queryBuilder.Term("_name", "work", key: "workcriteria");

			Console.WriteLine("\r\nSecond Criteria");

			var results2 = ExecuteAndAssert(queryBuilder, 8);

			Assert.AreNotEqual(results.TotalHits, results2.TotalHits);
		}

		[Test]
		public void SimpleWildCardQuery()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup(x => x.WildCard("_name", "h*"));

			ExecuteAndAssert(queryBuilder, 16);
		}

		[Test]
		public void ChainedTerms()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard("_name", "*highlights"),
					x => x.Term("_name", "work")
				);

			ExecuteAndAssert(queryBuilder, 4);
		}

		[Test]
		public void RemoveTerms()
		{
			
		}

		[Test]
		public void Group()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard("_name", "*highlights"),
					x => x.Term("_name", "work")
				);
			queryBuilder.Group().Setup
				(
					x => x.Term("_name", "work")
				);

			throw new NotImplementedException("Needs finishing");

			ExecuteAndAssert(queryBuilder, 4);
		}

		private TopDocs ExecuteAndAssert(IQueryBuilder queryBuilder, int numberOfHitsExpected)
		{
			TopDocs results = Search.Execute(queryBuilder.Build(), 20);

			foreach (Document document in Search.GetTopDocuments(results))
			{
				Console.WriteLine(document.GetValues("_name")[0]);
			}

			Assert.AreEqual(numberOfHitsExpected, results.TotalHits);
			
			return results;
		}
	}
}
