using System;
using System.Collections.Generic;
using AutoMapper;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucinq.Enums;
using Lucinq.Interfaces;
using Lucinq.Querying;
using NUnit.Framework;

namespace Lucinq.UnitTests.IntegrationTests
{
	[TestFixture]
	public class PrefixTests
	{
		/// <summary>
		/// Simple constructor, only job is to setup the mappings from document to strong type.
		/// </summary>
		public PrefixTests()
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
		/// Grab all articles that have titles that are prefixed with 'in pictures' and then perform
		/// a wildcard search against those. Pre-filter the documents with a date range
		/// </summary>
		[Test]
		public void InPicturesTest()
		{
			LuceneSearch luceneSearch = new LuceneSearch(GeneralConstants.Paths.BBCIndex);

			IQueryBuilder queryBuilder = new QueryBuilder();

			// The prefix query needs to work against a field that hasn't been tokenised/analysed to work.
			queryBuilder.Setup(
				x => x.PrefixedWith(BBCFields.Sortable, QueryParser.Escape("in pictures"), Matches.Always)
			);

			LuceneSearchResult result = luceneSearch.Execute(queryBuilder);
			List<NewsArticle> data = Mapper.Map<List<Document>, List<NewsArticle>>(result.GetTopDocuments());

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
			LuceneSearch luceneSearch = new LuceneSearch(GeneralConstants.Paths.BBCIndex);

			IQueryBuilder queryBuilder = new QueryBuilder();
			DateTime february = DateTime.Parse("01/01/2013");
			DateTime end = DateTime.Parse("31/01/2013");

			// The prefix query needs to work against a field that hasn't been tokenised/analysed to work.
			queryBuilder.Setup(
				x => x.PrefixedWith(BBCFields.Sortable, QueryParser.Escape("in pictures"), Matches.Always),
				x => x.WildCard(BBCFields.Description, "images", Matches.Always),
				x => x.Filter(DateRangeFilter.Filter(BBCFields.PublishDateObject, february, end))
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
