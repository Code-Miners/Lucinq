namespace Lucinq.Solr.UnitTests.IntegrationTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enums;
    using Core.Interfaces;
    using Core.Querying;
    using NUnit.Framework;
	using Querying;
    using SolrNet;


    [TestFixture]
	public class BasicTests : BaseTestFixture
	{
        private const string searchServiceName = "https://solr840:8987/solr";
        private const string indexName = "bbc_index";
		
        
        #region [ Properties ]

		[OneTimeSetUp]
		public void Setup()
		{
            Startup.Init<Dictionary<string, object>>("https://solr840:8987/solr/bbc_index");
		}

		#endregion

		[Test]
		public void Term()
		{
			SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
			IQueryBuilder queryBuilder = new QueryBuilder();

			queryBuilder.Term(BBCFields.Title, "africa");

			var results = ExecuteAndAssert(solrSearch, queryBuilder, 7);

			Assert.AreEqual(7, results.TotalHits);
		}


        [Test]
        public void DateRange()
        {

			SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();

            DateTime startDate = new DateTime(2012, 12, 1);
            DateTime endDate = new DateTime(2013, 1, 1);

            queryBuilder.TermRange("publish_date_time", TestHelpers.GetDateString(startDate), TestHelpers.GetDateString(endDate));

            ExecuteAndAssert(solrSearch, queryBuilder, 60);

        }

		private ISolrSearchResult ExecuteAndAssert(SolrSearch solrSearch, IQueryBuilder queryBuilder, int numberOfHitsExpected)
        {

            ISolrSearchResult result = solrSearch.Execute(queryBuilder);

            IList<Dictionary<string, object>> documents = result.GetTopItems();

            Console.WriteLine("Searched documents in {0} ms", result.ElapsedTimeMs);
            Console.WriteLine();

            WriteDocuments(documents);

            Assert.AreEqual(numberOfHitsExpected, result.TotalHits);

            return result;
        }

        [Test]
        public void SetupSyntax()
        {
			SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Setup(x => x.Term(BBCFields.Title, "africa"));

            ExecuteAndAssert(solrSearch, queryBuilder, 7);
        }




		private void WriteDocuments(ICollection<Dictionary<string, object>> documents)
        {
            int counter = 0;
            Console.WriteLine("Showing the first 30 docs");

            foreach (var document in documents)
            {
                if (counter >= 29)
                {
                    return;
                }

                Console.WriteLine("Title: " + document["title"]);
                Console.WriteLine("Description: " + document["description"]);
                Console.WriteLine("Publish Date: " + document["publish_date_time"]);
                Console.WriteLine("Url: " + document["link"]);
                Console.WriteLine();
                counter++;
            }
        }


        [Test]
        public void SimpleOrClauseSuccessful()
        {
			SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Or
            (
                x => x.Term(BBCFields.Title, "africa"),
                x => x.Term(BBCFields.Title, "europe")
            );

            ExecuteAndAssert(solrSearch, queryBuilder, 10);
        }

        [Test]
        public void SimpleAndClauseSuccessful()
        {

			SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.And
            (
                x => x.Term(BBCFields.Title, "africa"),
                x => x.Term(BBCFields.Title, "road")
            );

            ExecuteAndAssert(solrSearch, queryBuilder, 1);
        }


        [Test]
        public void RemoveAndReexecute()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Term(BBCFields.Title, "africa", key: "africacriteria");

            var results = ExecuteAndAssert(solrSearch, queryBuilder, 7);

            queryBuilder.Queries.Remove("africacriteria");
            queryBuilder.Term(BBCFields.Title, "report", key: "businesscriteria");

            Console.WriteLine("\r\nSecond Criteria");

            var results2 = ExecuteAndAssert(solrSearch, queryBuilder, 5);

            Assert.AreNotEqual(results.TotalHits, results2.TotalHits);
        }

        [Test]
        public void TermsOr()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Terms(BBCFields.Title, new[] { "europe", "africa" }, Matches.Sometimes);
            ExecuteAndAssert(solrSearch, queryBuilder, 10);
        }

        [Test]
        public void EasyOr()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Or(
                x => x.Term(BBCFields.Title, "europe"),
                x => x.Term(BBCFields.Title, "africa"));
            ExecuteAndAssert(solrSearch, queryBuilder, 10);
		}

        [Test]
        public void PhraseDistance()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Phrase(2, new[]
            {
                new KeyValuePair<string, string>(BBCFields.Title, "wildlife"),
                new KeyValuePair<string, string>(BBCFields.Title, "africa")
            });

            ExecuteAndAssert(solrSearch, queryBuilder, 1);
        }

        [Test]
        public void Fuzzy()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Fuzzy(BBCFields.Title, "afric", 0.5f);
            ExecuteAndAssert(solrSearch, queryBuilder, 16);
        }


        [Test]
        public void Paging()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Setup(x => x.WildCard(BBCFields.Description, "a*"));

            var results = ExecuteAndAssertPaged(solrSearch, queryBuilder, 1318, 0, 10);
            var documents = results.GetRange(0, 9);
            Assert.AreEqual(10, documents.Count);

            var results2 = ExecuteAndAssertPaged(solrSearch, queryBuilder, 1318, 1, 11);
            var documents2 = results2.GetRange(1, 10);
            Assert.AreEqual(10, documents2.Count);

            for (var i = 0; i < documents.Count - 1; i++)
            {
                Assert.AreEqual(documents2[i][BBCFields.Title], documents[i + 1][BBCFields.Title]);
            }

        }

        [Test]
        public void Sorting()
        {

            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Setup
            (
                x => x.WildCard(BBCFields.Description, "a*"),
                x => x.Sort("title_sort")
            );

            ISolrSearchResult result = ExecuteAndAssert(solrSearch, queryBuilder, 1318);
            var documents = result.GetRange(0, 100);
            for (var i = 1; i < documents.Count; i++)
            {
                string thisDocumentSortable = documents[i][BBCFields.Title].ToString();
                string lastDocumentSortable = documents[i - 1][BBCFields.Title].ToString();
                Assert.IsTrue(String.Compare(thisDocumentSortable, lastDocumentSortable, StringComparison.Ordinal) >= 0);
            }
        }


        [Test]
        public void MultipleSorting()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Setup
            (
                x => x.WildCard(BBCFields.Description, "a*"),
                x => x.Sort("title_sort"),
                x => x.Sort("secondary_sort")
            );

            ISolrSearchResult result = ExecuteAndAssert(solrSearch, queryBuilder, 1318);
            var documents = result.GetRange(0, 1000);
            for (var i = 1; i < documents.Count; i++)
            {
                string thisDocumentSortable = GetSecondarySortString(documents[i]);
                string lastDocumentSortable = GetSecondarySortString(documents[i - 1]);
                Assert.IsTrue(String.Compare(thisDocumentSortable, lastDocumentSortable, StringComparison.Ordinal) >= 0);
            }
        }

     
        private ISolrSearchResult ExecuteAndAssertPaged(SolrSearch solrSearch, IQueryBuilder queryBuilder, int numberOfHitsExpected, int start, int end)
        {
            // Search = new SolrSearch(GeneralConstants.Paths.BBCIndex);
            var result = solrSearch.Execute(queryBuilder);
            var documents = result.GetRange(start, end);

            Console.WriteLine("Searched documents in {0} ms", result.ElapsedTimeMs);
            Console.WriteLine();

            WriteDocuments(documents);

            Assert.AreEqual(numberOfHitsExpected, result.TotalHits);

            return result;
        }

        private string GetSecondarySortString(Dictionary<string, object> document)
        {
            return String.Format("{0}_{1}", document["secondary_sort"].ToString(), document["title_sort"].ToString());
        }


        [Test]
        public void TermsAnd()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Terms(BBCFields.Title, new[] { "africa", "road" }, Matches.Always);
            ExecuteAndAssert(solrSearch, queryBuilder, 1);
        }

        [Test]
        public void EasyAnd()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.And(
                x => x.Term(BBCFields.Title, "road"),
                x => x.Term(BBCFields.Title, "africa"));
            ExecuteAndAssert(solrSearch, queryBuilder, 1);
        }

        [Test]
        public void WildCard()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Setup(x => x.WildCard(BBCFields.Description, "a*"));

            ExecuteAndAssert(solrSearch, queryBuilder, 1318);
        }

        [Test]
        public void ChainedTerms()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Setup
            (
                x => x.WildCard(BBCFields.Description, "a*"),
                x => x.Term(BBCFields.Description, "police")
            );

            ExecuteAndAssert(solrSearch, queryBuilder, 25);
        }

        [Test]
        public void Group()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
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

            ExecuteAndAssert(solrSearch, queryBuilder, 5);
        }


        [Test]
        public void SpeedExample()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            Console.WriteLine("A simple test to show Lucene getting quicker as queries are done");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Pass 1");
            SpeedExampleExecute(solrSearch, "b");
            Console.WriteLine();

            Console.WriteLine("Pass 2");
            SpeedExampleExecute(solrSearch, "c");
            Console.WriteLine();

            Console.WriteLine("Pass 3");
            SpeedExampleExecute(solrSearch, "a");

            Console.WriteLine();
            Console.WriteLine("** Repeating Passes **");

            Console.WriteLine("Repeat Pass 1");
            SpeedExampleExecute(solrSearch, "b");
            Console.WriteLine();

            Console.WriteLine("Repeat Pass 2");
            SpeedExampleExecute(solrSearch, "c");
            Console.WriteLine();

            Console.WriteLine("Repeat Pass 3");
            SpeedExampleExecute(solrSearch, "a");
        }

        [Test]
        public void Enumerable()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Term(BBCFields.Title, "africa");

            var result = solrSearch.Execute(queryBuilder);
            /*WriteDocuments(result);*/
            Assert.AreEqual(7, result.Count());
        }

        [Test]
        public void EnumerableWithWhere()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Term(BBCFields.Title, "africa");

            var result = solrSearch.Execute(queryBuilder).Where(doc => doc[BBCFields.Title].ToString().IndexOf("your", StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
            WriteDocuments(result);
            Assert.AreEqual(1, result.Count());
        }
        public void SpeedExampleExecute(SolrSearch solrSearch, string startingCharacter)
        {
            // Chosen due to it being the slowest query

            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Setup
            (
                x => x.WildCard(BBCFields.Description, startingCharacter + "*"),
                x => x.Term(BBCFields.Description, "sport")
            );
            var result = solrSearch.Execute(queryBuilder);

            Console.WriteLine("Total Results: {0}", result.TotalHits);
            Console.WriteLine("Elapsed Time: {0}", result.ElapsedTimeMs);
        }
        [Test]
        public void Filter()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Term(BBCFields.Title, "Africa");
            queryBuilder.Filter(new LucinqFilter("description", "Close encounters from the heart of Africa", Comparator.Equals));

            var results = ExecuteAndAssert(solrSearch, queryBuilder, 1);

            Assert.AreEqual(1, results.TotalHits);
        }


        [Test]
        public void MultipleSortingDescending()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Setup
            (
                x => x.WildCard(BBCFields.Description, "a*"),
                x => x.Sort(BBCFields.SecondarySort, true),
                x => x.Sort("title_sort", true)
            );

            ISolrSearchResult result = ExecuteAndAssert(solrSearch, queryBuilder, 1318);
            var documents = result.GetRange(0, 1000);
            for (var i = 1; i < documents.Count; i++)
            {
                string thisDocumentSortable = GetSecondarySortString(documents[i]);
                string lastDocumentSortable = GetSecondarySortString(documents[i - 1]);
                Assert.IsTrue(String.Compare(lastDocumentSortable, thisDocumentSortable, StringComparison.Ordinal) >= 0);
            }
        }



        [Test]
        public void SortDescending()
        {
            SolrSearch solrSearch = new SolrSearch(new SolrSearchDetails(searchServiceName), indexName);
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Setup
            (
                x => x.WildCard(BBCFields.Description, "a*"),
                x => x.Sort("title_sort", true)
            );

            ISolrSearchResult result = ExecuteAndAssert(solrSearch, queryBuilder, 1318);
            var documents = result.GetRange(0, 10);
            for (var i = 1; i < documents.Count; i++)
            {
                string thisDocumentSortable = documents[i]["title_sort"].ToString();
                string lastDocumentSortable = documents[i - 1]["title_sort"].ToString();
                Assert.IsTrue(String.Compare(thisDocumentSortable, lastDocumentSortable, StringComparison.Ordinal) <= 0);
            }
        }
    }
}
