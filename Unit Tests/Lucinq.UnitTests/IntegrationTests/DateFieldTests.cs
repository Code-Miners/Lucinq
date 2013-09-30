using System;
using System.Collections.Generic;
using AutoMapper;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Building;
using Lucinq.Enums;
using Lucinq.Interfaces;
using Lucinq.Querying;
using NUnit.Framework;

namespace Lucinq.UnitTests.IntegrationTests
{
	[TestFixture]
	public class DateFieldTests
	{
		

		/// <summary>
		/// Simple constructor, only job is to setup the mappings from document to strong type.
		/// </summary>
		public DateFieldTests()
		{
			// Map the lucene document back to our news article.
			Mapper.CreateMap<Document, NewsArticle>()
				.ForMember(x => x.Title, opt => opt.MapFrom(y => y.GetValues(BBCFields.Title)[0]))
				.ForMember(x => x.Description, opt => opt.MapFrom(y => y.GetValues(BBCFields.Description)[0]))
				.ForMember(x => x.PublishDateTime, opt => opt.MapFrom(y => FromTicks(y.GetValues(BBCFields.PublishDateObject)[0])))
				.ForMember(x => x.Link, opt => opt.MapFrom(y => y.GetValues(BBCFields.Link)[0]))
				.ForMember(x => x.Copyright, opt => opt.Ignore());

			Mapper.AssertConfigurationIsValid();
		}

		/// <summary>
		/// Pull out all the articles within a particular date range and dump them to the error console.
		/// </summary>
		[Test]
		public void GetArticlesWithDateTest()
		{
			LuceneSearch luceneSearch = new LuceneSearch(GeneralConstants.Paths.BBCIndex);

			IQueryBuilder queryBuilder = new QueryBuilder();
			DateTime february = DateTime.Parse("01/02/2013");

			queryBuilder.Setup(
				x => x.DateRange(BBCFields.PublishDateObject, february, february.AddDays(28))
			);

			LuceneSearchResult result = luceneSearch.Execute(queryBuilder);
			List<NewsArticle> data = Mapper.Map<List<Document>, List<NewsArticle>>(result.GetTopDocuments());

			WriteDocuments(data);

            Console.WriteLine("Searched {0} documents in {1} ms", result.TotalHits, result.ElapsedTimeMs);
			Console.WriteLine();

			Assert.AreNotEqual(0, result.TotalHits);
		}

		/// <summary>
		/// Pull out all the articles within a particular date range and dump them to the error console.
		/// </summary>
		[Test]
		public void SearchArticlesWithDateFilterTest()
		{
			LuceneSearch luceneSearch = new LuceneSearch(GeneralConstants.Paths.DateIndex, true);

			IQueryBuilder queryBuilder = new QueryBuilder();
			DateTime february = DateTime.Parse("01/02/2013");
			DateTime end = DateTime.Parse("28/02/2013");

			queryBuilder.Setup(
				x => x.WildCard(BBCFields.Description, "food", Matches.Always),
				x => x.Filter(NumericRangeFilter.NewLongRange(BBCFields.PublishDate, february.Ticks, end.Ticks, true, true))
			);

			LuceneSearchResult result = luceneSearch.Execute(queryBuilder);
			List<NewsArticle> data = Mapper.Map<List<Document>, List<NewsArticle>>(result.GetTopDocuments());

			WriteDocuments(data);

			Console.WriteLine("Searched {0} documents in {1} ms", luceneSearch.IndexSearcher.MaxDoc, result.ElapsedTimeMs);
			Console.WriteLine();

			Assert.AreNotEqual(0, result.TotalHits);
		}

		/// <summary>
		/// Search all the articles within a particular date range and dump them to the error console.
		/// </summary>
		[Test]
		public void SearchArticlesWithinDateRangeTest()
		{
			LuceneSearch luceneSearch = new LuceneSearch(GeneralConstants.Paths.BBCIndex);

			IQueryBuilder queryBuilder = new QueryBuilder();
			DateTime month = DateTime.Parse("01/02/2013");

			queryBuilder.Setup(
				x => x.DateRange(BBCFields.PublishDateObject, month, month.AddDays(28)),
				x => x.WildCard(BBCFields.Description, "food")
			);

			LuceneSearchResult result = luceneSearch.Execute(queryBuilder);
			List<NewsArticle> data = Mapper.Map<List<Document>, List<NewsArticle>>(result.GetTopDocuments());

			WriteDocuments(data);

            Console.WriteLine("Searched {0} documents in {1} ms", result.TotalHits, result.ElapsedTimeMs);
			Console.WriteLine();

			Assert.AreNotEqual(0, result.TotalHits);
		}

		#region helper methods

		/// <summary>
		/// Helper to convert back from the value in the index to a DateTime
		/// </summary>
		/// <param name="ticks"></param>
		/// <returns></returns>
		private DateTime FromTicks(String ticks)
		{
			long temp;

			if (!Int64.TryParse(ticks, out temp))
			{
				return DateTime.MinValue;
			}

			return new DateTime(temp);
		}

		/// <summary>
		/// Dump the list of news articles to the error console
		/// </summary>
		/// <param name="documents"></param>
		private void WriteDocuments(List<NewsArticle> documents)
		{
			int counter = 0;
			Console.Error.WriteLine("Showing the first 30 docs");
			documents.ForEach(
				document =>
				{
					if (counter >= 29)
					{
						return;
					}

					Console.Error.WriteLine("Title: " + document.Title);
					Console.Error.WriteLine("Description: " + document.Description);
					Console.Error.WriteLine("Publish Date: " + document.PublishDateTime.ToShortDateString());
					Console.Error.WriteLine("Url: " + document.Link);
					Console.Error.WriteLine();
					counter++;
				}
			);
		}

		#endregion
	}
}
