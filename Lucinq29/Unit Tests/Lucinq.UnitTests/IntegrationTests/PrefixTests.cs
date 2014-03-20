using System;
using System.Collections.Generic;
using AutoMapper;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Store;
using Lucinq.Enums;
using Lucinq.Interfaces;
using Lucinq.Querying;
using NUnit.Framework;

namespace Lucinq.UnitTests.IntegrationTests
{
	[TestFixture]
	public class PrefixTests : BaseTestFixture
	{
		public PrefixTests()
		{
			
		}

		/// <summary>
		/// Grab all articles that have titles that are prefixed with 'in pictures' and then perform
		/// a wildcard search against those. Pre-filter the documents with a date range
		/// </summary>
		[Test]
		public void InPicturesTest()
		{
			LuceneSearch luceneSearch = new LuceneSearch(IndexDirectory);

			IQueryBuilder queryBuilder = new QueryBuilder();

			// The prefix query needs to work against a field that hasn't been tokenised/analysed to work.
			queryBuilder.Setup(
				x => x.PrefixedWith(BBCFields.Sortable, QueryParser.Escape("in pictures"), Matches.Always)
			);

            ILuceneSearchResult result = luceneSearch.Execute(queryBuilder);
			List<NewsArticle> data = Mapper.Map<List<Document>, List<NewsArticle>>(result.GetTopItems());

			WriteDocuments(data);

			Console.WriteLine("Searched {0} documents in {1} ms", result.TotalHits, result.ElapsedTimeMs);
			Console.WriteLine();

			Assert.AreNotEqual(0, result.TotalHits);
		}

		/// <summary>
		/// Grab all articles that have titles that are prefixed with 'in pictures' and then perform
		/// a wildcard search against those. Pre-filter the documents with a date range
		/// </summary>
		[Test]
		public void InPicturesWithSearchAndFilterTest()
		{
			LuceneSearch luceneSearch = new LuceneSearch(IndexDirectory);

			IQueryBuilder queryBuilder = new QueryBuilder();
			DateTime february = DateTime.Parse("01/01/2013");
			DateTime end = DateTime.Parse("31/01/2013");

			// The prefix query needs to work against a field that hasn't been tokenised/analysed to work.
			queryBuilder.Setup(
				x => x.PrefixedWith(BBCFields.Sortable, QueryParser.Escape("in pictures"), Matches.Always),
				x => x.WildCard(BBCFields.Description, "images", Matches.Always),
				x => x.Filter(DateRangeFilter.Filter(BBCFields.PublishDateObject, february, end))
			);

            ILuceneSearchResult result = luceneSearch.Execute(queryBuilder);
            List<NewsArticle> data = Mapper.Map<List<Document>, List<NewsArticle>>(result.GetTopItems());

			WriteDocuments(data);

			Console.WriteLine("Searched {0} documents in {1} ms", result.TotalHits, result.ElapsedTimeMs);
			Console.WriteLine();

			Assert.AreNotEqual(0, result.TotalHits);
		}
	}
}
