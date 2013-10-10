using System;
using System.Linq;
using Lucinq.SitecoreIntegration.DatabaseManagement;
using Lucinq.SitecoreIntegration.Querying;
using Lucinq.SitecoreIntegration.Querying.Interfaces;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Lucinq.SitecoreIntegration.UnitTests.IntegrationTests
{
	[TestFixture]
	public class QueryTests
	{
		#region [ Fields ]

		private SitecoreSearch search;

		#endregion

		#region [ Setup / Teardown ]

		[TestFixtureSetUp]
		public void Setup()
		{
			search = new SitecoreSearch(Sitecore.Configuration.Settings.IndexFolder + "\\lucinq_master_index", new DatabaseHelper("master"));
		}

		#endregion

		#region [ Template Tests ]

		[Test]
		public void GetByTemplateId()
		{
            ID templateId = SitecoreIds.AdvertTemplateId;

            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
			queryBuilder.Setup(x => x.TemplateId(templateId));
			// queryBuilder.TemplateId(templateId);

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder, 20);

			Assert.Greater(sitecoreSearchResult.LuceneSearchResult.TotalHits, 0);

			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 10);

			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
			Console.WriteLine("Sitecore Elapsed Time: {0}", sitecoreItemResult.ElapsedTimeMs);

			sitecoreItemResult.Items.ForEach(
					item =>
					{
						Console.WriteLine(item.Name);
                        Assert.AreEqual(SitecoreIds.AdvertTemplateId, item.TemplateID);
					});
			Assert.Greater(sitecoreItemResult.Items.Count, 0);
		}

		#endregion

        #region [ Enumerable Test ]

	    [Test]
	    public void GetEnumerableItems()
	    {
            ID templateId = SitecoreIds.AdvertTemplateId;

            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
			queryBuilder.Setup(x => x.TemplateId(templateId));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder, 20);

			Assert.Greater(sitecoreSearchResult.LuceneSearchResult.TotalHits, 0);

			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 10);

			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
			Console.WriteLine("Sitecore Elapsed Time: {0}", sitecoreItemResult.ElapsedTimeMs);

	        Assert.AreEqual(sitecoreItemResult.Items.FirstOrDefault(), sitecoreItemResult.FirstOrDefault());
            Assert.AreEqual(sitecoreItemResult.Items.Count, sitecoreItemResult.Count());
	    }

        #endregion

        #region [ Failure Tests ]

        [Test]
		public void MoreResultsThanLuceneHits()
		{
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
			queryBuilder.Setup(x => x.Field("page_title", "lucinq*"));
			// queryBuilder.TemplateId(templateId);

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder, 20);

			Assert.Greater(sitecoreSearchResult.LuceneSearchResult.TotalHits, 0);

			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 20);

			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
			Console.WriteLine("Sitecore Elapsed Time: {0}", sitecoreItemResult.ElapsedTimeMs);

			Assert.AreEqual(1, sitecoreItemResult.Items.Count);

			sitecoreItemResult.Items.ForEach(
					item => Console.WriteLine(item.Name));
			Assert.Greater(sitecoreItemResult.Items.Count, 0);
		}

		#endregion

		#region [ Id Tests ]

		[Test]
		public void GetById()
		{
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
            queryBuilder.Setup(x => x.Id(SitecoreIds.HomeItemId));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder);
			Assert.Greater(sitecoreSearchResult.TotalHits, 0);
			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 10);

			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
			Console.WriteLine("Sitecore Elapsed Time: {0}", sitecoreItemResult.ElapsedTimeMs);

			sitecoreItemResult.Items.ForEach(
				item =>
				{
					Console.WriteLine(item.Name);
                    Assert.AreEqual(SitecoreIds.HomeItemId, item.ID);
				});
			Assert.Greater(sitecoreItemResult.Items.Count, 0);
		}

		#endregion

		#region [ Name Tests ]

		[Test]
		public void GetByName()
		{
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
			queryBuilder.Setup(x => x.Name("ford"));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder);
			Assert.Greater(sitecoreSearchResult.TotalHits, 0);
			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 10);

			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
			Console.WriteLine("Sitecore Elapsed Time: {0}", sitecoreItemResult.ElapsedTimeMs);

			sitecoreItemResult.Items.ForEach(
				item =>
				{
					Console.WriteLine(item.Name);
					Assert.IsTrue(item.Name.IndexOf("ford", StringComparison.InvariantCultureIgnoreCase) >= 0);
				});
			Assert.Greater(sitecoreItemResult.Items.Count, 0);
		}

		[Test]
		public void GetByNameWildCard()
		{
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
			queryBuilder.Setup(x => x.Field("title", "t*"), x => x.Language(Language.Parse("en")));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder);
			Assert.Greater(sitecoreSearchResult.TotalHits, 0);
			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 100);

			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
			Console.WriteLine("Sitecore Elapsed Time: {0}", sitecoreItemResult.ElapsedTimeMs);

			sitecoreItemResult.Items.ForEach(
				item =>
					{
						Console.WriteLine(item.Name);
						Assert.IsTrue(item["title"].IndexOf("t", StringComparison.InvariantCultureIgnoreCase) >= 0);
					});
			Assert.Greater(sitecoreItemResult.Items.Count, 0);
		}

		[Ignore("For Load Testing Use Only")]
		[Test]
		public void RepeatedTest()
		{
			for (var i = 0; i < 1000; i++)
			{
				GetByName();
			}

			for (var i = 0; i < 1000; i++)
			{
				GetByNameWildCard();
			}
		}

		[Test]
		public void GetByLanguage()
		{
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
			Language language = Language.Parse("en");
			queryBuilder.Setup(x => x.Language(language));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder);
			Assert.Greater(sitecoreSearchResult.TotalHits, 0);
			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 100);

			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
			Console.WriteLine("Sitecore Elapsed Time: {0}", sitecoreItemResult.ElapsedTimeMs);

			sitecoreItemResult.Items.ForEach(
				item =>
				{
					Console.WriteLine(item.Name);
					Assert.AreEqual(language, item.Language);
				});
			Assert.Greater(sitecoreItemResult.Items.Count, 0);
		}

		#endregion

		#region [ Field ]

		[Test]
		public void GetByFieldValue()
		{
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
            queryBuilder.Setup(x => x.Field("page_title", "ford"));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder);
			Assert.Greater(sitecoreSearchResult.TotalHits, 0);
			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 10);

			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
			Console.WriteLine("Sitecore Elapsed Time: {0}", sitecoreItemResult.ElapsedTimeMs);

			sitecoreItemResult.Items.ForEach(
				item =>
				{
					Console.WriteLine(item.Name);
                    Assert.IsTrue(item["page title"].IndexOf("ford", StringComparison.InvariantCultureIgnoreCase) >= 0);
				});
			Assert.Greater(sitecoreItemResult.Items.Count, 0);
		}

		#endregion

		#region [ Single Item Tests ]

		[Test]
		public void GetFirstItem()
		{
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
			queryBuilder.Setup(x => x.Field("page_title", "ford"));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder);
			Assert.Greater(sitecoreSearchResult.TotalHits, 0);
			Item item = sitecoreSearchResult.GetItem();
			Console.WriteLine(item.Name);
			Assert.IsTrue(item["page title"].IndexOf("ford", StringComparison.InvariantCultureIgnoreCase) >= 0);
			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
		}

		[Test]
		public void GetItemOutsideIndex()
		{
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
			queryBuilder.Setup(x => x.Field("page_title", "ford"));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder);
			Assert.Greater(sitecoreSearchResult.TotalHits, 0);
			Item item = sitecoreSearchResult.GetItem(int.MaxValue);
			Assert.IsNull(item);
			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
		}

		#endregion

		#region [ Heirarchy Extensions ]


		[Test]
		public void GetByAncestor()
		{
			Ancestor();
		}

		private void Ancestor(bool displayOutput = true)
		{
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
			queryBuilder.Setup(x => x.DescendantOf(SitecoreIds.MakesPageId));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder);

			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 9);

			Assert.Greater(sitecoreSearchResult.TotalHits, 0);

			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
			Console.WriteLine("Sitecore Elapsed Time: {0}", sitecoreItemResult.ElapsedTimeMs);

			if (displayOutput)
			{
				sitecoreItemResult.Items.ForEach(
					item => Console.WriteLine(item.Name));
			}

			Assert.Greater(sitecoreItemResult.Items.Count, 0);
		}


		[Test]
		public void GetByParent()
		{
            SitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
            queryBuilder.Setup(x => x.ChildOf(SitecoreIds.HomeItemId));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder);
			Assert.Greater(sitecoreSearchResult.TotalHits, 0);
			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 9);

			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
			Console.WriteLine("Sitecore Elapsed Time: {0}", sitecoreItemResult.ElapsedTimeMs);

			sitecoreItemResult.Items.ForEach(
				item =>
				{
					Console.WriteLine(item.Name);
                    Assert.AreEqual(SitecoreIds.HomeItemId, item.Parent.ID);
				});
			Assert.Greater(sitecoreItemResult.Items.Count, 0);
		}

		[Test]
		public void GetFromDerivedTemplate()
		{
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();

            queryBuilder.Setup(x => x.TemplateDescendsFrom(SitecoreIds.AdvertTemplateId));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder);
			Assert.Greater(sitecoreSearchResult.TotalHits, 0);
			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 9);

			Console.WriteLine("Lucene Elapsed Time: {0}", sitecoreSearchResult.ElapsedTimeMs);
			Console.WriteLine("Sitecore Elapsed Time: {0}", sitecoreItemResult.ElapsedTimeMs);

			sitecoreItemResult.Items.ForEach(
				item => Console.WriteLine(item.Name));
			Assert.Greater(sitecoreItemResult.Items.Count, 0);
		}

		#endregion

		#region [ Performance Tests ]

		[Test]
		public void RepeatAncestorTests()
		{
			Console.WriteLine("The return from sitecore and lucene should get quicker");
			Console.WriteLine();

			Console.WriteLine("Pass 1");
			Ancestor(false);
			Console.WriteLine();

			Console.WriteLine("Pass 2");
			Ancestor(false);
			Console.WriteLine();

			Console.WriteLine("Pass 3");
			Ancestor(false);
		}
		#endregion

		#region [ Paging Tests ]
		
		[Test]
		public  void Paging()
		{
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
			queryBuilder.Setup(x => x.DescendantOf(SitecoreIds.MakesPageId));

			ISitecoreSearchResult sitecoreSearchResult = search.Execute(queryBuilder);

			ISitecoreItemResult sitecoreItemResult = sitecoreSearchResult.GetPagedItems(0, 9);

			Assert.AreEqual(10, sitecoreItemResult.Items.Count);

			ISitecoreItemResult sitecoreItemResult2 = sitecoreSearchResult.GetPagedItems(0, 19);

			Assert.AreEqual(20, sitecoreItemResult2.Items.Count);

			ISitecoreItemResult sitecoreItemResult3 = sitecoreSearchResult.GetPagedItems(10, 29);

			Assert.AreEqual(20, sitecoreItemResult3.Items.Count);
		}
		#endregion
	}
}
