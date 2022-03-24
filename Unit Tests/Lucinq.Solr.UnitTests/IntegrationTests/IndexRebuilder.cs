namespace Lucinq.Solr.UnitTests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Xml.Linq;
    using CommonServiceLocator;
    using NUnit.Framework;
    using SolrNet;
    using SolrNet.Attributes;
    using SolrNet.Impl;
    using Directory = System.IO.Directory;

    [TestFixture]
    /*[Ignore("Index Rebuilding is specific, run the individual tests.")]*/
    public class IndexRebuilder
    {
        #region [ BBC News Indexing ]

        private const string searchServiceName = "https://solr840:8987/solr";
        private const string indexName = "bbc_index";

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
        /*[Ignore("Don't build this under normal unit test conditions")]*/
        [Test]
        public void BuildBBCIndex()
        {
            // IF YOU WANT TO RUN THIS, DELETE THE CONTENTS OF THE EXISTING INDEX FIRST, OTHERWISE, IT WILL APPEND

            Startup.Init<Dictionary<string, object>>("https://solr840:8987/solr/bbc_index");

            /*
            var headerParser = ServiceLocator.Current.GetInstance<ISolrHeaderResponseParser>();
            var statusParser = ServiceLocator.Current.GetInstance<ISolrStatusResponseParser>();
            ISolrCoreAdmin solrCoreAdmin =
                new SolrCoreAdmin(new SolrConnection(searchServiceName), headerParser, statusParser);
            solrCoreAdmin.Create(coreName: indexName, instanceDir: indexName);*/


            string[] rssFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\" + GeneralConstants.Paths.RSSFeed);
            int count = 0;
            List<NewsArticle> newsArticles = new List<NewsArticle>();
            foreach (var rssFile in rssFiles)
            {
                string secondarySort = count % 2 == 0 ? "A" : "B";
                count++;
                List<NewsArticle> feedArticles = ReadFeed(rssFile);
                newsArticles.AddRange(feedArticles);

            }

            foreach (var newsArticle in newsArticles)
            {
                newsArticle.Key = count.ToString();
                count++;
            }



            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Dictionary<string, object>>>();
            int counter = 0;
            foreach (var newsArticle in newsArticles)
            {
                string secondarySort = counter % 2 == 0 ? "A" : "B";
                counter++;
                var item = new Dictionary<string, object>
                {
                    { "_uniqueid", newsArticle.Key ?? "" },
                    { "title", newsArticle.Title ?? "" },
                    { "link", newsArticle.Link ?? "" },
                    { "copyright", newsArticle.Copyright ?? "" },
                    { "description", newsArticle.Description ?? ""},
                    { "publish_date_time", TestHelpers.GetDateString(newsArticle.PublishDateTime) },
                    { "file_name", newsArticle.FileName ?? "" },
                    { "secondary_sort", secondarySort  }

                };

                solr.Add(item);
                solr.Commit();
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

        #endregion


        public class CarDataItem
        {
            public Guid AdvertId { get; set; }
            public DateTime Created { get; set; }
            public int MakeId { get; set; }
            public int ModelId { get; set; }
            public string Code { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public string Variant { get; set; }
            public string Options { get; set; }
            public int AdvertTypeId { get; set; }
            public string AdvertType { get; set; }
            public int AdvertStatusId { get; set; }
            public string AdvertStatus { get; set; }
            public string Town { get; set; }
            public string County { get; set; }
            public decimal Price { get; set; }
            public decimal? Mileage { get; set; }
            public string FuelType { get; set; }
            public string Postcode { get; set; }
        }

        public class NewsArticle
        {

            [SolrUniqueKey("_uniqueid")] public string Key { get; set; }

            [SolrField("title")] public string Title { get; set; }

            [SolrField("link")] public string Link { get; set; }

            [SolrField("copyright")] public string Copyright { get; set; }

            [SolrField("description")] public string Description { get; set; }

            [SolrField("publish_date_time")] public DateTime PublishDateTime { get; set; }

            [SolrField("file_name")] public string FileName { get; set; }
        }

    }
}