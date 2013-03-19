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
using NUnit.Framework;
using Directory = System.IO.Directory;
using Version = Lucene.Net.Util.Version;

namespace Lucinq.UnitTests.IntegrationTests
{
	[TestFixture]
	[Ignore]
	public class IndexRebuilder
	{
		[Test]
		public void ReadFeedAndDisplay()
		{
			var newsArticles = ReadFeed(String.Format("{0}{1}", GeneralConstants.Paths.RSSFeed, @"\Business.xml"));
			foreach (var newsArticle in newsArticles)
			{
				Console.WriteLine(newsArticle.Title);
				Console.WriteLine(newsArticle.Description);
				Console.WriteLine(newsArticle.Copyright);
				Console.WriteLine(newsArticle.Link);
				Console.WriteLine(TestHelpers.GetDateString(newsArticle.PublishDateTime));
				Console.WriteLine();
			}
		}

		/// <summary>
		/// Before you say - not robust - just does enough to get the job done ;)
		/// </summary>
		[Test]
		public void BuildIndex()
		{
			var indexFolder = FSDirectory.Open(new DirectoryInfo(GeneralConstants.Paths.BBCIndex));

			Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
			using (IndexWriter indexWriter = new IndexWriter(indexFolder, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
			{
				string[] rssFiles = Directory.GetFiles(GeneralConstants.Paths.RSSFeed);
				// int count = 0;
				foreach (var rssFile in rssFiles)
				{
					/*if (count > 4)
					{
						break;
					}
					count ++;*/
					var newsArticles = ReadFeed(rssFile);
					newsArticles.ForEach(
						newsArticle => 
							indexWriter.AddDocument
							(
								x => x.AddAnalysedField(BBCFields.Title, newsArticle.Title, true),
								x => x.AddAnalysedField(BBCFields.Description, newsArticle.Description, true),
								x => x.AddAnalysedField(BBCFields.Copyright, newsArticle.Copyright),
								x => x.AddStoredField(BBCFields.Link, newsArticle.Link),
								x => x.AddNonAnalysedField(BBCFields.PublishDate, TestHelpers.GetDateString(newsArticle.PublishDateTime), true),
								x => x.AddNonAnalysedField(BBCFields.Sortable, newsArticle.Title)) // must be non-analysed to sort against it
							);
				}

				indexWriter.Optimize();
				indexWriter.Close();
			}
		}

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
	}

	public class NewsArticle
	{
		public string Title { get; set; }
		public string Link { get; set; }
		public string Copyright { get; set; }
		public string Description { get; set; }
		public DateTime PublishDateTime { get; set; }
	}
}
