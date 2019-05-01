namespace Lucinq.Solr.UnitTests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using AutoMapper;
    using Core.Interfaces;
    using Core.Querying;
    using Lucene.Net.Documents;
    using Lucene30.Querying;
    using NUnit.Framework;

    [TestFixture]
	public class DateFieldTests : BaseTestFixture
	{
		/// <summary>
		/// Pull out all the articles within a particular date range and dump them to the error console.
		/// </summary>
		[Test]
		public void GetArticlesWithDateTest()
		{
			LuceneSearch luceneSearch = new LuceneSearch(IndexDirectory);


			IQueryBuilder queryBuilder = new QueryBuilder();
			DateTime february = DateTime.Parse("01/02/2013");

			queryBuilder.Setup(
				x => x.DateRange(BBCFields.PublishDateObject, february, february.AddDays(28))
			);

			ILuceneSearchResult result = luceneSearch.Execute(queryBuilder);
			IList<NewsArticle> data = Mapper.Map<IList<Document>, IList<NewsArticle>>(result.GetTopItems());

			WriteDocuments(data);

            Console.WriteLine("Searched {0} documents in {1} ms", result.TotalHits, result.ElapsedTimeMs);
			Console.WriteLine();

			Assert.AreNotEqual(0, result.TotalHits);
		}

        // todo: NM - fix filter
        /*
		/// <summary>
		/// Pull out all the articles within a particular date range and dump them to the error console.
		/// </summary>
		[Test]
		public void SearchArticlesWithDateFilterTest()
		{
			LuceneSearch luceneSearch = new LuceneSearch(IndexDirectory);

			IQueryBuilder queryBuilder = new QueryBuilder();
			DateTime february = DateTime.Parse("01/02/2013");
			DateTime end = DateTime.Parse("28/02/2013");

			queryBuilder.Setup(
				x => x.WildCard(BBCFields.Description, "food", Matches.Always),
				x => x.Filter(DateRangeFilter.Filter(BBCFields.PublishDateObject, february, end))
			);

            ILuceneSearchResult result = luceneSearch.Execute(queryBuilder);
            IList<NewsArticle> data = Mapper.Map<IList<Document>, IList<NewsArticle>>(result.GetTopItems());

			WriteDocuments(data);

			Console.WriteLine("Searched {0} documents in {1} ms", result.TotalHits, result.ElapsedTimeMs);
			Console.WriteLine();

			Assert.AreNotEqual(0, result.TotalHits);
		}
        */

		/// <summary>
		/// Search all the articles within a particular date range and dump them to the error console.
		/// </summary>
		[Test]
		public void SearchArticlesWithinDateRangeTest()
		{
			LuceneSearch luceneSearch = new LuceneSearch(IndexDirectory);

			IQueryBuilder queryBuilder = new QueryBuilder();
			DateTime month = DateTime.Parse("01/02/2013");

			queryBuilder.Setup(
				x => x.DateRange(BBCFields.PublishDateObject, month, month.AddDays(28)),
				x => x.WildCard(BBCFields.Description, "food")
			);

            ILuceneSearchResult result = luceneSearch.Execute(queryBuilder);
            IList<NewsArticle> data = Mapper.Map<IList<Document>, IList<NewsArticle>>(result.GetTopItems());

			WriteDocuments(data);

            Console.WriteLine("Searched {0} documents in {1} ms", result.TotalHits, result.ElapsedTimeMs);
			Console.WriteLine();

			Assert.AreNotEqual(0, result.TotalHits);
		}
	}
}