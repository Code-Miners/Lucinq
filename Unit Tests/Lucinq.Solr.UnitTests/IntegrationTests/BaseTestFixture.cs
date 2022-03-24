namespace Lucinq.Solr.UnitTests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using AutoMapper;
    using NUnit.Framework;
    using SolrNet.Mapping;

    [TestFixture]
    public abstract class BaseTestFixture
    {
        /// <summary>
        /// Simple constructor, only job is to setup the mappings from document to strong type.
        /// </summary>
        protected BaseTestFixture()
        {
        }

        [SetUp]
        public void TestFixtureSetup()
        {
                 }

        [TearDown]
        public void TestFixtureTearDown()
        {        }

        /// <summary>
        protected List<IndexRebuilder.NewsArticle> ReadFeed(string filePath)
        {
            List<IndexRebuilder.NewsArticle> newsArticles = new List<IndexRebuilder.NewsArticle>();
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
                    IndexRebuilder.NewsArticle newsArticle = new IndexRebuilder.NewsArticle();
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
        protected void WriteDocuments(IList<IndexRebuilder.NewsArticle> documents)
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
