using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.Azure.Search.Models;
using NUnit.Framework;

namespace Lucinq.AzureSearch.UnitTests.IntegrationTests
{
	[TestFixture]
	public abstract class BaseTestFixture
	{
		/// <summary>
		/// Simple constructor, only job is to setup the mappings from document to strong type.
		/// </summary>
		protected BaseTestFixture()
		{
			// Map the lucene document back to our news article.
			Mapper.CreateMap<SearchResult, NewsArticle>()
				.ForMember(x => x.Title, opt => opt.MapFrom(y => y.Document[BBCFields.Title].ToString()))
				.ForMember(x => x.Description, opt => opt.MapFrom(y => y.Document[BBCFields.Description].ToString()))
				.ForMember(x => x.PublishDateTime, opt => opt.MapFrom(y => FromTicks(y.Document[BBCFields.PublishDateTime].ToString())))
				.ForMember(x => x.Link, opt => opt.MapFrom(y => y.Document[BBCFields.Link].ToString()))
                .ForMember(x => x.FileName, opt => opt.MapFrom(y => y.Document[BBCFields.FileName].ToString()))
                .ForMember(x => x.Copyright, opt => opt.Ignore());

			Mapper.AssertConfigurationIsValid();
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
