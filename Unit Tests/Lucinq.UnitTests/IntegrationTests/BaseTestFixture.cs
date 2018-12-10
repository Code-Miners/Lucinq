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
using Lucinq.Lucene30.Building;
using NUnit.Framework;
using Version = Lucene.Net.Util.Version;

namespace Lucinq.Lucene30.UnitTests.IntegrationTests
{
	[TestFixture]
	public abstract class BaseTestFixture
	{
		protected RAMDirectory IndexDirectory { get; set; }

		/// <summary>
		/// Simple constructor, only job is to setup the mappings from document to strong type.
		/// </summary>
		protected BaseTestFixture()
		{
			// Map the lucene document back to our news article.
			Mapper.CreateMap<Document, NewsArticle>()
				.ForMember(x => x.Title, opt => opt.MapFrom(y => y.GetValues(BBCFields.Title)[0]))
				.ForMember(x => x.Description, opt => opt.MapFrom(y => y.GetValues(BBCFields.Description)[0]))
				.ForMember(x => x.PublishDateTime, opt => opt.MapFrom(y => FromTicks(y.GetValues(BBCFields.PublishDateObject)[0])))
				.ForMember(x => x.Link, opt => opt.MapFrom(y => y.GetValues(BBCFields.Link)[0]))
                .ForMember(x => x.FileName, opt => opt.MapFrom(y => y.GetValues(BBCFields.FileName)[0]))
                .ForMember(x => x.Copyright, opt => opt.Ignore());

			Mapper.AssertConfigurationIsValid();
		}

		[SetUp]
		public void TestFixtureSetup()
		{
			IndexDirectory = new RAMDirectory();
			BuildInMemoryIndex(IndexDirectory);
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
			IndexDirectory.Dispose();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="directory"></param>
		public void BuildInMemoryIndex(RAMDirectory directory)
		{
			Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
			using (IndexWriter indexWriter = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
			{
				string[] rssFiles = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\" + GeneralConstants.Paths.RSSFeed);
				int count = 0;
				foreach (var rssFile in rssFiles)
				{
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
								x => x.AddNonAnalysedField(BBCFields.PublishDateString, TestHelpers.GetDateString(newsArticle.PublishDateTime), true),
								x => x.AddNonAnalysedField(BBCFields.PublishDateObject, newsArticle.PublishDateTime, true),
								x => x.AddNonAnalysedField(BBCFields.Sortable, newsArticle.Title, true), // must be non-analysed to sort against it
								x => x.AddNonAnalysedField(BBCFields.SecondarySort, secondarySort, true),
                                x => x.AddStoredField(BBCFields.FileName, rssFile))

							);
				}

				indexWriter.Optimize();
				indexWriter.Dispose();
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

		#region helper methods

		/// <summary>
		/// Helper to convert back from the value in the index to a DateTime
		/// </summary>
		/// <param name="ticks"></param>
		/// <returns></returns>
		protected DateTime FromTicks(String ticks)
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
		protected void WriteDocuments(IList<NewsArticle> documents)
		{
			int counter = 0;
			Console.Error.WriteLine("Showing the first 30 docs");
			documents.Each(
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
