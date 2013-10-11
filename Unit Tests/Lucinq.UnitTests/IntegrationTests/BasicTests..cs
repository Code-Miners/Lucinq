using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucinq.Enums;
using Lucinq.Extensions;
using Lucinq.Interfaces;
using Lucinq.Querying;
using NUnit.Framework;

namespace Lucinq.UnitTests.IntegrationTests
{
	[TestFixture]
	public class BasicTests : BaseTestFixture
	{
		#region [ Properties ]

		[TestFixtureSetUp]
		public void Setup()
		{
		}

		#endregion

		[Test]
		public void Term()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Term(BBCFields.Title, "africa");

			var results = ExecuteAndAssert(luceneSearch, queryBuilder, 8);

			Assert.AreEqual(8, results.TotalHits);
		}


		[Test]
		public void TermRange()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();

			DateTime startDate = new DateTime(2012, 12, 1);
			DateTime endDate = new DateTime(2013, 1, 1);

            queryBuilder.TermRange(BBCFields.PublishDateString, TestHelpers.GetDateString(startDate), TestHelpers.GetDateString(endDate));

			ExecuteAndAssert(luceneSearch, queryBuilder, 60);

		}

		[Test]
		public void SetupSyntax()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup(x => x.Term(BBCFields.Title, "africa"));

			ExecuteAndAssert(luceneSearch, queryBuilder, 8);
		}

		[Test]
		public void SimpleOrClauseSuccessful()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Or
				(
					x => x.Term(BBCFields.Title, "africa"),
					x => x.Term(BBCFields.Title, "europe")
				);

			ExecuteAndAssert(luceneSearch, queryBuilder, 12);
		}

		[Test]
		public void SimpleAndClauseSuccessful()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.And
				(
					x => x.Term(BBCFields.Title, "africa"),
					x => x.Term(BBCFields.Title, "road")
				);

			ExecuteAndAssert(luceneSearch, queryBuilder, 1);
		}

		[Test]
		public void RemoveAndReexecute()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Term(BBCFields.Title, "africa", key: "africacriteria");

			var results = ExecuteAndAssert(luceneSearch, queryBuilder, 8);

			queryBuilder.Queries.Remove("africacriteria");
			queryBuilder.Term(BBCFields.Title, "report", key: "businesscriteria");

			Console.WriteLine("\r\nSecond Criteria");

			var results2 = ExecuteAndAssert(luceneSearch, queryBuilder, 5);

			Assert.AreNotEqual(results.TotalHits, results2.TotalHits);
		}

		[Test]
		public void EasyOr()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Terms(BBCFields.Title, new[] {"europe", "africa"}, Matches.Sometimes);
			ExecuteAndAssert(luceneSearch, queryBuilder, 12);
		}

		/*[Test]
		public void SimpleNot()
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Not().Term("_name", "home");
			var results = ExecuteAndAssert(queryBuilder, 12);
		}*/

		[Test]
		public void PhraseDistance()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Phrase(2).AddTerm(BBCFields.Title, "wildlife").AddTerm(BBCFields.Title, "africa");
			var results = ExecuteAndAssert(luceneSearch, queryBuilder, 1);
		}

		[Test]
		public void Fuzzy()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Fuzzy(BBCFields.Title, "afric");
			var results = ExecuteAndAssert(luceneSearch, queryBuilder, 16);
		}

		[Test]
		public void Paging()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup(x => x.WildCard(BBCFields.Description, "a*"));

			var results = ExecuteAndAssertPaged(luceneSearch, queryBuilder, 902, 0, 10);
			var documents = results.GetPagedDocuments(0, 9);
			Assert.AreEqual(10, documents.Count);

			var results2 = ExecuteAndAssertPaged(luceneSearch, queryBuilder, 902, 1, 11);
			var documents2 = results2.GetPagedDocuments(1, 10);
			Assert.AreEqual(10, documents2.Count);

			for (var i = 0; i < documents.Count - 1; i++)
			{
				Assert.AreEqual(documents2[i].GetValues(BBCFields.Title).FirstOrDefault(), documents[i+1].GetValues(BBCFields.Title).FirstOrDefault());
			}
				
		}

		[Test]
		public void Sorting()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "a*"),
					x => x.Sort(BBCFields.Sortable)
				);

			ILuceneSearchResult result = ExecuteAndAssert(luceneSearch, queryBuilder, 902);
			List<Document> documents = result.GetPagedDocuments(0, 100);
			for (var i = 1; i < documents.Count; i++)
			{
				string thisDocumentSortable = documents[i].GetValues(BBCFields.Sortable).FirstOrDefault();
				string lastDocumentSortable = documents[i - 1].GetValues(BBCFields.Sortable).FirstOrDefault();
				Assert.IsTrue(String.Compare(thisDocumentSortable, lastDocumentSortable, StringComparison.Ordinal) >= 0);
			}
		}

		[Test]
		public void MultipleSorting()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "a*"),
					x => x.Sort(BBCFields.SecondarySort),
					x => x.Sort(BBCFields.Sortable)
				);

			ILuceneSearchResult result = ExecuteAndAssert(luceneSearch, queryBuilder, 902);
			List<Document> documents = result.GetPagedDocuments(0, 1000);
			for (var i = 1; i < documents.Count; i++)
			{
				string thisDocumentSortable = GetSecondarySortString(documents[i]);
				string lastDocumentSortable = GetSecondarySortString(documents[i - 1]);
				Assert.IsTrue(String.Compare(thisDocumentSortable, lastDocumentSortable, StringComparison.Ordinal) >= 0);
			}
		}

		[Test]
		public void MultipleSortingDescending()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "a*"),
					x => x.Sort(BBCFields.SecondarySort, true),
					x => x.Sort(BBCFields.Sortable, true)
				);

			ILuceneSearchResult result = ExecuteAndAssert(luceneSearch, queryBuilder, 902);
			List<Document> documents = result.GetPagedDocuments(0, 1000);
			for (var i = 1; i < documents.Count; i++)
			{
				string thisDocumentSortable = GetSecondarySortString(documents[i]);
				string lastDocumentSortable = GetSecondarySortString(documents[i - 1]);
				Assert.IsTrue(String.Compare(lastDocumentSortable, thisDocumentSortable, StringComparison.Ordinal) >= 0);
			}
		}

		private string GetSecondarySortString(Document document)
		{
			return String.Format("{0}_{1}", document.GetValues(BBCFields.SecondarySort).FirstOrDefault(), document.GetValues(BBCFields.Sortable).FirstOrDefault());	
		}

		[Test]
		public void SortDescending()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "a*"),
					x => x.Sort(BBCFields.Sortable, true)
				);

			ILuceneSearchResult result = ExecuteAndAssert(luceneSearch, queryBuilder, 902);
			List<Document> documents = result.GetPagedDocuments(0, 10);
			for (var i = 1; i < documents.Count; i++)
			{
				string thisDocumentSortable = documents[i].GetValues(BBCFields.Sortable).FirstOrDefault();
				string lastDocumentSortable = documents[i - 1].GetValues(BBCFields.Sortable).FirstOrDefault();
				Assert.IsTrue(String.Compare(thisDocumentSortable, lastDocumentSortable, StringComparison.Ordinal) <= 0);
			}
		}

		[Test]
		public void EasyAnd()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Terms(BBCFields.Title, new[] { "africa", "road" }, occur: Matches.Always);
			ExecuteAndAssert(luceneSearch, queryBuilder, 1);
		}

		[Test]
		public void WildCard()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup(x => x.WildCard(BBCFields.Description, "a*"));

			ExecuteAndAssert(luceneSearch, queryBuilder, 902);
		}

		[Test]
		public void ChainedTerms()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "a*"),
					x => x.Term(BBCFields.Description, "police")
				);

			ExecuteAndAssert(luceneSearch, queryBuilder, 17);
		}

		[Test]
		public void Group()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Title, "africa"),
					x => x.Group().Setup
							(
								y => y.Term(BBCFields.Description, "africa", Matches.Sometimes),
                                y => y.Term(BBCFields.Description, "amazing", Matches.Sometimes)
							)
				);

			ExecuteAndAssert(luceneSearch, queryBuilder, 5);
		}

		[Test]
		public void SpeedExample()
		{
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
			Console.WriteLine("A simple test to show Lucene getting quicker as queries are done");
			Console.WriteLine("----------------------------------------------------------------");
			Console.WriteLine();
			Console.WriteLine("Pass 1");
			SpeedExampleExecute(luceneSearch, "b");
			Console.WriteLine();

			Console.WriteLine("Pass 2");
			SpeedExampleExecute(luceneSearch, "c");
			Console.WriteLine();

			Console.WriteLine("Pass 3");
			SpeedExampleExecute(luceneSearch, "a");

			Console.WriteLine();
			Console.WriteLine("** Repeating Passes **");

			Console.WriteLine("Repeat Pass 1");
			SpeedExampleExecute(luceneSearch, "b");
			Console.WriteLine();

			Console.WriteLine("Repeat Pass 2");
			SpeedExampleExecute(luceneSearch, "c");
			Console.WriteLine();

			Console.WriteLine("Repeat Pass 3");
			SpeedExampleExecute(luceneSearch, "a");
		}

	    [Test]
        public void Enumerable()
        {
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Term(BBCFields.Title, "africa");

            var result = luceneSearch.Execute(queryBuilder);
            WriteDocuments(result);
            Assert.AreEqual(8, result.Count());
	    }

        [Test]
        public void EnumerableWithWhere()
        {
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearchProvider(IndexDirectory));
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Term(BBCFields.Title, "africa");

            var result = luceneSearch.Execute(queryBuilder).Where(doc => doc.GetField(BBCFields.Title).StringValue().IndexOf("your", StringComparison.OrdinalIgnoreCase) >= 0);
            WriteDocuments(result);
            Assert.AreEqual(1, result.Count());
        }

		public void SpeedExampleExecute(LuceneSearch luceneSearch, string startingCharacter)
		{
			// Chosen due to it being the slowest query

			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, startingCharacter + "*"),
					x => x.Term(BBCFields.Description, "sport")
				);
			var result = luceneSearch.Execute(queryBuilder);

			Console.WriteLine("Total Results: {0}", result.TotalHits);
			Console.WriteLine("Elapsed Time: {0}", result.ElapsedTimeMs);
		}

		private ILuceneSearchResult ExecuteAndAssert(LuceneSearch luceneSearch, IQueryBuilder queryBuilder, int numberOfHitsExpected)
		{
			var result = luceneSearch.Execute(queryBuilder);

            var documents = result.GetTopDocuments();

			Console.WriteLine("Searched documents in {0} ms", result.ElapsedTimeMs);
			Console.WriteLine();

			WriteDocuments(documents);

			Assert.AreEqual(numberOfHitsExpected, result.TotalHits);
			
			return result;
		}

		private ILuceneSearchResult ExecuteAndAssertPaged(LuceneSearch luceneSearch, IQueryBuilder queryBuilder, int numberOfHitsExpected, int start, int end)
		{
			// Search = new LuceneSearch(GeneralConstants.Paths.BBCIndex);
			var result = luceneSearch.Execute(queryBuilder);
			List<Document> documents = result.GetPagedDocuments(start, end);

            Console.WriteLine("Searched documents in {0} ms", result.ElapsedTimeMs);
			Console.WriteLine();

			WriteDocuments(documents);

			Assert.AreEqual(numberOfHitsExpected, result.TotalHits);

			return result;
		}

		private void WriteDocuments(IEnumerable<Document> documents)
		{
			int counter = 0;
			Console.WriteLine("Showing the first 30 docs");
		    foreach (var document in documents)
		    {
		       if (counter >= 29)
					{
						return;
					}
					Console.WriteLine("Title: " + document.GetValues(BBCFields.Title)[0]);
					Console.WriteLine("Secondary Sort:" + document.GetValues(BBCFields.SecondarySort)[0]);
					Console.WriteLine("Description: " + document.GetValues(BBCFields.Description)[0]);
                    Console.WriteLine("Publish Date: " + document.GetValues(BBCFields.PublishDateString)[0]);
					Console.WriteLine("Url: "+ document.GetValues(BBCFields.Link)[0]);
					Console.WriteLine();
					counter++; 
		    }
		}
	}
}
