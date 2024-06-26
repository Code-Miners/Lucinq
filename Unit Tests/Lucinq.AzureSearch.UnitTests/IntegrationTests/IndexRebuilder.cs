﻿using System;
using System.Data.SqlClient;
using NUnit.Framework;

namespace Lucinq.AzureSearch.UnitTests.IntegrationTests
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Azure.Search;
    using Microsoft.Azure.Search.Models;

    [TestFixture]
	//[Ignore ("Index Rebuilding is specific, run the individual tests.")]
	public class IndexRebuilder
    {

        private const string indexName = "bbc-index";

        #region [ BBC News Indexing ]

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

		/// <summary>
		/// Before you say - not robust - just does enough to get the job done ;)
		/// </summary>
		//[Ignore("Don't build this under normal unit test conditions")]
        [Test]
		public void BuildBBCIndex()
		{
			// IF YOU WANT TO RUN THIS, DELETE THE CONTENTS OF THE EXISTING INDEX FIRST, OTHERWISE, IT WILL APPEND

            SearchServiceClient client = new SearchServiceClient("yourServiceHere", new SearchCredentials("yourApiKeyHere"));

			string[] rssFiles = Directory.GetFiles(GeneralConstants.Paths.RSSFeed);
		    int count = 0;
		    List<NewsArticle> newsArticles = new List<NewsArticle>();

            foreach (var rssFile in rssFiles)
			{
				string secondarySort = count%2 == 0 ? "A" : "B";
				count ++;
				List<NewsArticle> feedArticles = ReadFeed(rssFile);
                newsArticles.AddRange(feedArticles);

			}

		    foreach (var newsArticle in newsArticles)
		    {
		        newsArticle.Key = count.ToString();
		        count++;
		    }

		    var definition = new Index()
		    {
		        Name = indexName,
		        Fields = FieldBuilder.BuildForType<NewsArticle>()
		    };

		    client.Indexes.Create(definition);
		    ISearchIndexClient indexClient = client.Indexes.GetClient(indexName);

		    var actions = newsArticles.Select(IndexAction.MergeOrUpload);
            
		    var batch = IndexBatch.New(actions.Skip(0).Take(500));

		    indexClient.Documents.Index(batch);

		    batch = IndexBatch.New(actions.Skip(500).Take(500));

		    indexClient.Documents.Index(batch);

		    batch = IndexBatch.New(actions.Skip(1000).Take(500));

		    indexClient.Documents.Index(batch);


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
        /*
        [Ignore("Indexing is not part of the normal tests")]
        [Test]
	    public void BuildCarDataIndex()
	    {
            var carDataItems = GetCarDataItems();

            if (carDataItems.Count == 0)
            {
                Assert.Fail("No Car Data Items Were Found");
            }

            if (Directory.Exists(GeneralConstants.Paths.CarDataIndex))
		    {
                Directory.Delete(GeneralConstants.Paths.CarDataIndex, true);
		    }

			var indexFolder = FSDirectory.Open(new DirectoryInfo(GeneralConstants.Paths.CarDataIndex));

			Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
	        using (IndexWriter indexWriter = new IndexWriter(indexFolder, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
	        {
	            foreach (var originalDataItem in carDataItems)
	            {
	                var carDataItem = originalDataItem;
	                indexWriter.AddDocument(
	                    x => x.AddAnalysedField(CarDataFields.AdvertId, carDataItem.AdvertId.ToString().ToLower()),
	                    x => x.AddNonAnalysedField(CarDataFields.MakeId, carDataItem.MakeId),
	                    x => x.AddNonAnalysedField(CarDataFields.ModelId, carDataItem.ModelId),
	                    x => x.AddNonAnalysedField(CarDataFields.Code, carDataItem.Code),
	                    x => x.AddAnalysedField(CarDataFields.Make, carDataItem.Make),
	                    x => x.AddAnalysedField(CarDataFields.Model, carDataItem.Make),
	                    x => x.AddAnalysedField(CarDataFields.Options, carDataItem.Options),
	                    x => x.AddNonAnalysedField(CarDataFields.AdvertTypeId, carDataItem.AdvertTypeId),
	                    x => x.AddAnalysedField(CarDataFields.AdvertType, carDataItem.AdvertType),
	                    x => x.AddNonAnalysedField(CarDataFields.IsLive, carDataItem.AdvertStatusId > 1 ? "1" : "0"),
	                    x => x.AddNonAnalysedField(CarDataFields.DateCreated, carDataItem.Created),
	                    x => x.AddAnalysedField(CarDataFields.Town, carDataItem.Town),
	                    x => x.AddAnalysedField(CarDataFields.County, carDataItem.County),
	                    x => x.AddAnalysedField(CarDataFields.Postcode, carDataItem.Postcode),
	                    x => x.AddAnalysedField(CarDataFields.FuelType, carDataItem.FuelType),
	                    x => x.AddNonAnalysedField(CarDataFields.Mileage, carDataItem.Mileage.ToString()),
	                    x => x.AddAnalysedField(CarDataFields.Price, carDataItem.Price.ToString(CultureInfo.InvariantCulture), true)
	                    );
	            }
	        }
	    }

	    private static List<CarDataItem> GetCarDataItems()
	    {
	        List<CarDataItem> carDataItems = new List<CarDataItem>();

	        using (
	            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CarData"].ConnectionString)
	            )
	        {
	            using (SqlCommand command = new SqlCommand("usp_LucinqTestData", connection))
	            {
	                connection.Open();
	                SqlDataReader reader = command.ExecuteReader();
	                while (reader.Read())
	                {
	                    // ignore data with no price
	                    if (reader["Price"] == DBNull.Value)
	                    {
	                        continue;
	                    }
	                    CarDataItem item = new CarDataItem
	                    {
	                        AdvertId = (Guid) reader["AdvertId"],
	                        Created = (DateTime) reader["DateCreated"],
	                        MakeId = (int) reader["MakeId"],
	                        ModelId = (int) reader["ModelId"],
	                        Code = reader["Code"].ToString(),
	                        Make = reader["Make"].ToString(),
	                        Model = reader["Model"].ToString(),
	                        Variant = reader.GetValueOrDefault<string>("Variant"),
	                        Options = reader.GetValueOrDefault<string>("Options"),
	                        FuelType = reader.GetValueOrDefault<string>("FuelType"),
	                        AdvertTypeId = (int) reader["AdvertTypeId"],
	                        AdvertType = reader["AdvertType"].ToString(),
	                        AdvertStatusId = (int) reader["AdvertStatusId"],
	                        AdvertStatus = reader["AdvertStatus"].ToString(),
	                        Town = reader.GetValueOrDefault<string>("Town"),
	                        County = reader.GetValueOrDefault<string>("County"),
	                        Postcode = reader.GetValueOrDefault<string>("Postcode"),
	                        Price = reader.GetValueOrDefault<decimal>("Price"),
	                        Mileage = reader.GetValueOrDefault<decimal?>("Mileage")
	                    };
	                    carDataItems.Add(item);
	                }
	            }
	        }
	        return carDataItems;
	    }*/
    }
    
   
    [SerializePropertyNamesAsCamelCase]
	public class NewsArticle
	{

        [System.ComponentModel.DataAnnotations.Key]
        public string Key { get; set; }

        [IsFilterable, IsSortable, IsSearchable]
		public string Title { get; set; }
        [IsFilterable, IsSortable, IsSearchable]
	    public string Link { get; set; }
        [IsFilterable, IsSortable, IsSearchable]
	    public string Copyright { get; set; }
	    [IsFilterable, IsSortable, IsSearchable]
            public string Description { get; set; }
	    [IsFilterable, IsSortable]
            public DateTime PublishDateTime { get; set; }
	    [IsFilterable, IsSortable, IsSearchable]
            public string FileName { get; set; }
	}

    public static class ReaderExtensions
    {
        public static T GetValueOrDefault<T>(this SqlDataReader reader, string fieldName)
        {
            if (reader[fieldName] == DBNull.Value)
            {
                return default(T);
            }
            return (T)reader[fieldName];
        }
    }
}