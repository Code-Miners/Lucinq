using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
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
		#region build index methods
		[Test]
		public void BuildIndex()
		{
			var indexFolder = FSDirectory.Open(new DirectoryInfo(GeneralConstants.Paths.DateIndex));

			Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
			using (IndexWriter indexWriter = new IndexWriter(indexFolder, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
			{
				String[] rssFiles = System.IO.Directory.GetFiles(GeneralConstants.Paths.RSSFeed);
				int count = 0;
				foreach (var rssFile in rssFiles)
				{
					/*if (count > 4)
					{
						break;
					}*/
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

		private static readonly LuceneSearch memorySearch = new LuceneSearch(GeneralConstants.Paths.DateIndex, true);
		private static LuceneSearch filesystemSearch = new LuceneSearch(GeneralConstants.Paths.DateIndex);
		static LuceneSearch[] searches = new[] { memorySearch };

		[TestFixtureSetUp]
		public void Setup()
		{
			//filesystemSearch = new LuceneSearch(GeneralConstants.Paths.DateIndex);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			//filesystemSearch.Dispose();
		}

		[Test]
		[TestCaseSource("searches")]
		public void GetArticlesWithDateTest(LuceneSearch luceneSearch)
		{
			IQueryBuilder queryBuilder = new QueryBuilder();

			DateTime february = DateTime.Parse("01/02/2013");

			queryBuilder.Setup(
				x => x.NumericRange(BBCFields.PublishDate, february.Ticks, february.AddDays(28).Ticks)
			);

			Console.Error.WriteLine(queryBuilder.ToString());
			//Console.Error.WriteLine(queryBuilder.);

			var result = luceneSearch.Execute(queryBuilder);

			var documents = result.GetTopDocuments();
			WriteDocuments(documents);

			Console.WriteLine("Searched {0} documents in {1} ms", luceneSearch.IndexSearcher.MaxDoc(), result.ElapsedTimeMs);
			Console.WriteLine();
	

			Assert.AreNotEqual(0, result.TotalHits);

		}

		private void WriteDocuments(List<Document> documents)
		{
			int counter = 0;
			Console.WriteLine("Showing the first 30 docs");
			documents.ForEach(
				document =>
				{
					if (counter >= 29)
					{
						return;
					}
					Console.WriteLine("Title: " + document.GetValues(BBCFields.Title)[0]);
					Console.WriteLine("Secondary Sort:" + document.GetValues(BBCFields.SecondarySort)[0]);
					Console.WriteLine("Description: " + document.GetValues(BBCFields.Description)[0]);
					Console.WriteLine("Publish Date: " + document.GetValues(BBCFields.PublishDate)[0]);
					Console.WriteLine("Url: " + document.GetValues(BBCFields.Link)[0]);
					Console.WriteLine();
					counter++;
				}
			);
		}
	}
}
