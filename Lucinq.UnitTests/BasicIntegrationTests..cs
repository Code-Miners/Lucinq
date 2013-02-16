using System;
using Lucene.Net.Documents;
using Lucene.Net.Search;
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
			BooleanQuery query = new BooleanQuery();

			query.Term("_name", "work");

			var results = ExecuteAndAssert(query, 8);

			Assert.AreEqual(8, results.TotalHits);

			BooleanQuery alternative = new BooleanQuery();
			alternative.Where(x => x.Term("_name", "work"));

			var results2 = Search.Execute(query, 20);
			Assert.AreEqual(results.TotalHits, results2.TotalHits);
		}

		[Test]
		public void SingleTermSetupSuccessful()
		{
			BooleanQuery query = new BooleanQuery();
			query.Setup(x => x.Term("_name", "work"));

			ExecuteAndAssert(query, 8);
		}

		[Test]
		public void SimpleOrClauseSuccessful()
		{
			BooleanQuery query = new BooleanQuery();

			query.Or
				(
					x => x.Term("_name", "work"),
					x => x.Term("_name", "text")
				);

			ExecuteAndAssert(query, 16);
		}

		[Test]
		public void SimpleAndClauseSuccessful()
		{
			BooleanQuery query = new BooleanQuery();

			query.And
				(
					x => x.Term("_name", "work"),
					x => x.Term("_name", "highlights")
				);

			ExecuteAndAssert(query, 4);
		}

		[Test]
		public void SimpleWildCardQuery()
		{
			BooleanQuery query = new BooleanQuery();
			query.Setup(x => x.WildCard("_name", "h*"));

			ExecuteAndAssert(query, 16);
		}

		[Test]
		public void ChainedTerms()
		{
			BooleanQuery query = new BooleanQuery();
			query.Setup
				(
					x => x.WildCard("_name", "*highlights"),
					x => x.Term("_name", "work")
				);

			ExecuteAndAssert(query, 4);
		}

		[Test]
		public void Group()
		{
			BooleanQuery query = new BooleanQuery();
			query.Setup
				(
					x => x.WildCard("_name", "*highlights"),
					x => x.Term("_name", "work")
				);
			query.Group().Setup
				(
					x => x.Term("_name", "work")
				);

			throw new NotImplementedException("Needs finishing");

			ExecuteAndAssert(query, 4);
		}

		private TopDocs ExecuteAndAssert(BooleanQuery query, int numberOfHitsExpected)
		{
			TopDocs results = Search.Execute(query, 20);

			foreach (Document document in Search.GetTopDocuments(results))
			{
				Console.WriteLine(document.GetValues("_name")[0]);
			}

			Assert.AreEqual(numberOfHitsExpected, results.TotalHits);
			
			return results;
		}
	}
}
