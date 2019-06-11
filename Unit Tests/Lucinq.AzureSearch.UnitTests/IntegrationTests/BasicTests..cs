using System;
using System.Collections.Generic;
using System.Linq;
using Lucinq.AzureSearch.Querying;
using Lucinq.Core.Enums;
using Lucinq.Core.Interfaces;
using Lucinq.Core.Querying;
using Microsoft.Azure.Search.Models;
using NUnit.Framework;

namespace Lucinq.AzureSearch.UnitTests.IntegrationTests
{
	[TestFixture]
	public class BasicTests : BaseTestFixture
	{
	    private const string adminApiKey = "yourApiKeyHere";
	    private const string searchServiceName = "yourServiceHere";
	    private const string indexName = "bbc-index";

        #region [ Properties ]

        [OneTimeSetUp]
		public void Setup()
		{
		}

		#endregion

		[Test]
		public void Term()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Term(BBCFields.Title, "africa");

			var results = ExecuteAndAssert(azureSearch, queryBuilder, 7);

			Assert.AreEqual(7, results.TotalHits);
		}

        [Test]
        [Ignore("Conceptual test for proving index closure")]
	    public void IndexUsed()
	    {
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Term(CarDataFields.Make, "ford");

            var searchResult = azureSearch.Execute(queryBuilder);

            Assert.AreEqual(8, searchResult.TotalHits);
	    }


		[Test]
		public void TermRange()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();

			DateTime startDate = new DateTime(2012, 12, 1);
			DateTime endDate = new DateTime(2013, 1, 1);

            queryBuilder.TermRange(BBCFields.PublishDateTime, TestHelpers.GetDateString(startDate), TestHelpers.GetDateString(endDate));

			ExecuteAndAssert(azureSearch, queryBuilder, 60);

		}

		[Test]
		public void SetupSyntax()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup(x => x.Term(BBCFields.Title, "africa"));

			ExecuteAndAssert(azureSearch, queryBuilder, 7);
		}

		[Test]
		public void SimpleOrClauseSuccessful()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Or
				(
					x => x.Term(BBCFields.Title, "africa"),
					x => x.Term(BBCFields.Title, "europe")
				);

			ExecuteAndAssert(azureSearch, queryBuilder, 10);
		}

		[Test]
		public void SimpleAndClauseSuccessful()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.And
				(
					x => x.Term(BBCFields.Title, "africa"),
					x => x.Term(BBCFields.Title, "road")
				);

			ExecuteAndAssert(azureSearch, queryBuilder, 1);
		}

		[Test]
		public void RemoveAndReexecute()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Term(BBCFields.Title, "africa", key: "africacriteria");

			var results = ExecuteAndAssert(azureSearch, queryBuilder, 7);

			queryBuilder.Queries.Remove("africacriteria");
			queryBuilder.Term(BBCFields.Title, "report", key: "businesscriteria");

			Console.WriteLine("\r\nSecond Criteria");

			var results2 = ExecuteAndAssert(azureSearch, queryBuilder, 5);

			Assert.AreNotEqual(results.TotalHits, results2.TotalHits);
		}

		[Test]
		public void TermsOr()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Terms(BBCFields.Title, new[] {"europe", "africa"}, Matches.Sometimes);
			ExecuteAndAssert(azureSearch, queryBuilder, 10);
		}

        [Test]
        public void EasyOr()
        {
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Or(
                x => x.Term(BBCFields.Title, "europe"),
                x => x.Term(BBCFields.Title, "africa"));
            ExecuteAndAssert(azureSearch, queryBuilder, 10);
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
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Phrase(2, new[]
		    {
		        new KeyValuePair<string, string>(BBCFields.Title, "wildlife"),
		        new KeyValuePair<string, string>(BBCFields.Title, "africa")
		    });

			ExecuteAndAssert(azureSearch, queryBuilder, 1);
		}

		[Test]
		public void Fuzzy()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Fuzzy(BBCFields.Title, "afric", 0.5f);
			ExecuteAndAssert(azureSearch, queryBuilder, 16);
		}

		[Test]
		public void Paging()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup(x => x.WildCard(BBCFields.Description, "a*"));

			var results = ExecuteAndAssertPaged(azureSearch, queryBuilder, 1318, 0, 10);
            var documents = results.GetRange(0, 9);
			Assert.AreEqual(10, documents.Count);

			var results2 = ExecuteAndAssertPaged(azureSearch, queryBuilder, 1318, 1, 11);
			var documents2 = results2.GetRange(1, 10);
			Assert.AreEqual(10, documents2.Count);

			for (var i = 0; i < documents.Count - 1; i++)
			{
				Assert.AreEqual(documents2[i].Document[BBCFields.Title].ToString(), documents[i+1].Document[BBCFields.Title].ToString());
			}
				
		}

		[Test]
		public void Sorting()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "a*"),
					x => x.Sort(BBCFields.Title)
				);

            IAzureSearchResult result = ExecuteAndAssert(azureSearch, queryBuilder, 1318);
            IList<SearchResult> documents = result.GetRange(0, 100);
			for (var i = 1; i < documents.Count; i++)
			{
				string thisDocumentSortable = documents[i].Document[BBCFields.Title].ToString();
				string lastDocumentSortable = documents[i - 1].Document[BBCFields.Title].ToString();
				Assert.IsTrue(String.Compare(thisDocumentSortable, lastDocumentSortable, StringComparison.Ordinal) >= 0);
			}
		}

		[Test]
		public void MultipleSorting()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "a*"),
					x => x.Sort(BBCFields.SecondarySort),
					x => x.Sort(BBCFields.Title)
				);

            IAzureSearchResult result = ExecuteAndAssert(azureSearch, queryBuilder, 1318);
            IList<SearchResult> documents = result.GetRange(0, 1000);
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
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "a*"),
					x => x.Sort(BBCFields.SecondarySort, true),
					x => x.Sort(BBCFields.Title, true)
				);

            IAzureSearchResult result = ExecuteAndAssert(azureSearch, queryBuilder, 1318);
            IList<SearchResult> documents = result.GetRange(0, 1000);
			for (var i = 1; i < documents.Count; i++)
			{
				string thisDocumentSortable = GetSecondarySortString(documents[i]);
				string lastDocumentSortable = GetSecondarySortString(documents[i - 1]);
				Assert.IsTrue(String.Compare(lastDocumentSortable, thisDocumentSortable, StringComparison.Ordinal) >= 0);
			}
		}

		private string GetSecondarySortString(SearchResult document)
		{
			return String.Format("{0}_{1}", document.Document[BBCFields.SecondarySort], document.Document[BBCFields.Title]);	
		}

		[Test]
		public void SortDescending()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "a*"),
					x => x.Sort(BBCFields.Title, true)
				);

            IAzureSearchResult result = ExecuteAndAssert(azureSearch, queryBuilder, 1318);
            IList<SearchResult> documents = result.GetRange(0, 10);
			for (var i = 1; i < documents.Count; i++)
			{
				string thisDocumentSortable = documents[i].Document[BBCFields.Title].ToString();
				string lastDocumentSortable = documents[i - 1].Document[BBCFields.Title].ToString();
				Assert.IsTrue(String.Compare(thisDocumentSortable, lastDocumentSortable, StringComparison.Ordinal) <= 0);
			}
		}

		[Test]
		public void TermsAnd()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Terms(BBCFields.Title, new[] { "africa", "road" }, Matches.Always);
			ExecuteAndAssert(azureSearch, queryBuilder, 1);
		}

        [Test]
        public void EasyAnd()
        {
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.And(
                x => x.Term(BBCFields.Title, "road"),
                x => x.Term(BBCFields.Title, "africa"));
            ExecuteAndAssert(azureSearch, queryBuilder, 1);
        }

		[Test]
		public void WildCard()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup(x => x.WildCard(BBCFields.Description, "a*"));

			ExecuteAndAssert(azureSearch, queryBuilder, 1318);
		}

		[Test]
		public void ChainedTerms()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "a*"),
					x => x.Term(BBCFields.Description, "police")
				);

			ExecuteAndAssert(azureSearch, queryBuilder, 25);
		}

		[Test]
		public void Group()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
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

			ExecuteAndAssert(azureSearch, queryBuilder, 5);
		}

		[Test]
		public void SpeedExample()
		{
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
			Console.WriteLine("A simple test to show Lucene getting quicker as queries are done");
			Console.WriteLine("----------------------------------------------------------------");
			Console.WriteLine();
			Console.WriteLine("Pass 1");
			SpeedExampleExecute(azureSearch, "b");
			Console.WriteLine();

			Console.WriteLine("Pass 2");
			SpeedExampleExecute(azureSearch, "c");
			Console.WriteLine();

			Console.WriteLine("Pass 3");
			SpeedExampleExecute(azureSearch, "a");

			Console.WriteLine();
			Console.WriteLine("** Repeating Passes **");

			Console.WriteLine("Repeat Pass 1");
			SpeedExampleExecute(azureSearch, "b");
			Console.WriteLine();

			Console.WriteLine("Repeat Pass 2");
			SpeedExampleExecute(azureSearch, "c");
			Console.WriteLine();

			Console.WriteLine("Repeat Pass 3");
			SpeedExampleExecute(azureSearch, "a");
		}

	    [Test]
        public void Enumerable()
        {
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Term(BBCFields.Title, "africa");

            var result = azureSearch.Execute(queryBuilder);
            WriteDocuments(result);
            Assert.AreEqual(7, result.Count());
	    }

        [Test]
        public void EnumerableWithWhere()
        {
            var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Term(BBCFields.Title, "africa");

            var result = azureSearch.Execute(queryBuilder).Where(doc => doc.Document[BBCFields.Title].ToString().IndexOf("your", StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
            WriteDocuments(result);
            Assert.AreEqual(1, result.Count());
        }

		public void SpeedExampleExecute(Querying.AzureSearch azureSearch, string startingCharacter)
		{
			// Chosen due to it being the slowest query

			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, startingCharacter + "*"),
					x => x.Term(BBCFields.Description, "sport")
				);
			var result = azureSearch.Execute(queryBuilder);

			Console.WriteLine("Total Results: {0}", result.TotalHits);
			Console.WriteLine("Elapsed Time: {0}", result.ElapsedTimeMs);
		}

	    [Test]
	    public void Filter()
	    {
	        var azureSearch = new Querying.AzureSearch(new AzureSearchDetails(searchServiceName, adminApiKey), indexName);
	        IQueryBuilder queryBuilder = new QueryBuilder();

	        queryBuilder.Term(BBCFields.Title, "africa");
            queryBuilder.Filter(new LucinqFilter("description", "Close encounters from the heart of Africa", Comparator.Equals));

            var results = ExecuteAndAssert(azureSearch, queryBuilder, 1);

	        Assert.AreEqual(1, results.TotalHits);
	    }

        private IAzureSearchResult ExecuteAndAssert(Querying.AzureSearch azureSearch, IQueryBuilder queryBuilder, int numberOfHitsExpected)
		{
			var result = azureSearch.Execute(queryBuilder);

            var documents = result.GetTopItems();

			Console.WriteLine("Searched documents in {0} ms", result.ElapsedTimeMs);
			Console.WriteLine();

			WriteDocuments(documents);

			Assert.AreEqual(numberOfHitsExpected, result.TotalHits);
			
			return result;
		}

		private IAzureSearchResult ExecuteAndAssertPaged(Querying.AzureSearch azureSearch, IQueryBuilder queryBuilder, int numberOfHitsExpected, int start, int end)
		{
			// Search = new AzureSearch(GeneralConstants.Paths.BBCIndex);
			var result = azureSearch.Execute(queryBuilder);
            IList<SearchResult> documents = result.GetRange(start, end);

            Console.WriteLine("Searched documents in {0} ms", result.ElapsedTimeMs);
			Console.WriteLine();

			WriteDocuments(documents);

			Assert.AreEqual(numberOfHitsExpected, result.TotalHits);

			return result;
		}

		private void WriteDocuments(IEnumerable<SearchResult> documents)
		{
			int counter = 0;
			Console.WriteLine("Showing the first 30 docs");
		    foreach (var document in documents)
		    {
		       if (counter >= 29)
					{
						return;
					}
					Console.WriteLine("Title: " + document.Document[BBCFields.Title]);
					//Console.WriteLine("Secondary Sort:" + document.Document[BBCFields.SecondarySort]);
					Console.WriteLine("Description: " + document.Document[BBCFields.Description]);
                    Console.WriteLine("Publish Date: " + document.Document[BBCFields.PublishDateTime]);
					Console.WriteLine("Url: "+ document.Document[BBCFields.Link]);
					Console.WriteLine();
					counter++; 
		    }
		}
	}
}
