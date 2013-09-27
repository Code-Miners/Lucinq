using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using AutoMapper;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucinq.Building;
using Lucinq.Interfaces;
using Lucinq.Querying;
using NUnit.Framework;
using Version = Lucene.Net.Util.Version;

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
				.ForMember(x => x.PublishDateTime, opt => opt.MapFrom(y => FromTicks(y.GetValues(BBCFields.PublishDate)[0])))
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
			LuceneSearch luceneSearch = new LuceneSearch(GeneralConstants.Paths.DateIndex, true);

			IQueryBuilder queryBuilder = new QueryBuilder();
			DateTime february = DateTime.Parse("01/02/2013");

			queryBuilder.Setup(
				x => x.DateRange(BBCFields.PublishDate, february, february.AddDays(28))
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
			LuceneSearch luceneSearch = new LuceneSearch(GeneralConstants.Paths.DateIndex, true);

			IQueryBuilder queryBuilder = new QueryBuilder();
			DateTime month = DateTime.Parse("01/02/2013");

			queryBuilder.Setup(
				x => x.DateRange(BBCFields.PublishDate, month, month.AddDays(28)),
				x => x.WildCard(BBCFields.Description, "food")
			);

			LuceneSearchResult result = luceneSearch.Execute(queryBuilder);
			List<NewsArticle> data = Mapper.Map<List<Document>, List<NewsArticle>>(result.GetTopDocuments());

			WriteDocuments(data);

			Console.WriteLine("Searched {0} documents in {1} ms", luceneSearch.IndexSearcher.MaxDoc, result.ElapsedTimeMs);
			Console.WriteLine();

			Assert.AreNotEqual(0, result.TotalHits);
		}

		#region build index methods
		[Test]
		public void BuildIndex()
		{
			FSDirectory indexFolder = FSDirectory.Open(new DirectoryInfo(GeneralConstants.Paths.DateIndex));

			Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
			using (IndexWriter indexWriter = new IndexWriter(indexFolder, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
			{
				String[] rssFiles = System.IO.Directory.GetFiles(GeneralConstants.Paths.RSSFeed);
				int count = 0;
				foreach (var rssFile in rssFiles)
				{
					Console.Error.WriteLine("Adding record.");
					string secondarySort = count % 2 == 0 ? "A" : "B";
					count++;
					var newsArticles = ReadFeed(rssFile);
					newsArticles.ForEach(
						newsArticle =>
							indexWriter.AddDocument
							(
								x => x.AddAnalysedField(BBCFields.Title, newsArticle.Title, true),
								x => x.AddAnalysedField(BBCFields.Description, newsArticle.Description, true),
								x => x.AddAnalysedField(BBCFields.Copyright, newsArticle.Copyright),
								x => x.AddStoredField(BBCFields.Link, newsArticle.Link),
								x => x.AddNonAnalysedField(BBCFields.PublishDate, newsArticle.PublishDateTime, true),
								x => x.AddNonAnalysedField(BBCFields.Sortable, newsArticle.Title, true), // must be non-analysed to sort against it
								x => x.AddNonAnalysedField(BBCFields.SecondarySort, secondarySort, true))
							);
				}

				indexWriter.Optimize();
				indexWriter.Close();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		protected List<NewsArticle> ReadFeed(string filePath)
		{
			List<NewsArticle> newsArticles = new List<NewsArticle>();
			using (var fileStream = File.OpenText(filePath))
			{
				var element = XElement.Load(fileStream);
				var channel = element.Element("channel");
				if (channel == null)
				{
					return null;
				}
				foreach (var node in channel.Elements("item"))
				{
					NewsArticle newsArticle = new NewsArticle();
					XElement descriptionElement = node.Element("description");
					if (descriptionElement != null)
					{
						newsArticle.Description = descriptionElement.Value.Trim();
					}

					var titleElement = node.Element("title");
					if (titleElement != null)
					{
						newsArticle.Title = titleElement.Value;
					}

					var linkElement = node.Element("link");
					if (linkElement != null)
					{
						newsArticle.Link = linkElement.Value;
					}

					var publishDateElement = node.Element("pubDate");
					if (publishDateElement != null)
					{
						newsArticle.PublishDateTime = DateTime.Parse(publishDateElement.Value);
					}
					newsArticles.Add(newsArticle);
				}
			}
			return newsArticles;
		}

		#endregion

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
