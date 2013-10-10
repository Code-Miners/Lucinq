using System;
using Glass.Mapper;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.CastleWindsor;
using Lucinq.GlassMapper.Sitecore.UnitTests.App_Start;
using Lucinq.GlassMapper.Sitecore.UnitTests.Objects;
using Lucinq.GlassMapper.SitecoreIntegration;
using Lucinq.GlassMapper.SitecoreIntegration.Extensions;
using Lucinq.Interfaces;
using Lucinq.Querying;
using Lucinq.SitecoreIntegration.Constants;
using Lucinq.SitecoreIntegration.Querying;
using Lucinq.SitecoreIntegration.Querying.Interfaces;
using NUnit.Framework;

namespace Lucinq.GlassMapper.Sitecore.UnitTests.IntegrationTests
{
	[TestFixture]
	public class GlassIntegrationTests
	{
	    private Context _context;
	    private LuceneSearch search;
	    private ISitecoreService service;

		[TestFixtureSetUp]
		public void Setup()
		{
            var resolver = DependencyResolver.CreateStandardResolver();
            GlassMapperScCustom.CastleConfig(resolver.Container);
            _context = Context.Create(resolver);
            _context.Load(GlassMapperScCustom.GlassLoaders());
            GlassMapperScCustom.PostLoad();
            _context.Load();
            search = new LuceneSearch(global::Sitecore.Configuration.Settings.IndexFolder + "\\lucinq_master_index");
            service = new SitecoreService("master");
		}


        #region [ Mapping Tests ]

		[Test]
		public void GetAttributeField()
		{
			string fieldName = GlassSitecoreExtensions.GetFieldName<GlassAttributeTestClass>(x => x.Text, service);
			Console.Write(fieldName);
			Assert.AreEqual("Text", fieldName);

			fieldName = GlassSitecoreExtensions.GetFieldName<GlassAttributeTestClass>(x => x.Language, service);
			Assert.AreEqual(SitecoreFields.Language, fieldName);

			fieldName = GlassSitecoreExtensions.GetFieldName<GlassAttributeTestClass>(x => x.DifferentName, service);
			Assert.AreEqual("Different Name", fieldName);

			fieldName = GlassSitecoreExtensions.GetFieldName<GlassAttributeTestClass>(x => x.Id, service);
			Assert.AreEqual(SitecoreFields.Id, fieldName);
		}

        #endregion

        #region [ Integration Tests ]

        [Test]
	    public void GetItemsUsingGlass()
	    {
            ISitecoreQueryBuilder queryBuilder = new SitecoreQueryBuilder();
            queryBuilder.Field<PageContent>(t => t.PageTitle, "ford");
            ISearchResult<PageContent> sitecoreSearchResult = new GlassSearchResult<PageContent>(service, search.Execute(queryBuilder));
            IItemResult<PageContent> result = sitecoreSearchResult.GetPagedItems(0, 19);
            Assert.AreEqual(20, result.Items.Count);
            foreach (PageContent item in result)
            {
                Assert.IsTrue(item.PageTitle.IndexOf("ford", StringComparison.InvariantCultureIgnoreCase) >= 0);
                Console.WriteLine("Page Title: " + item.PageTitle);
                Console.WriteLine("Page Subtitle: " + item.PageSubtitle);
                Console.WriteLine("Id: " + item.Id);
            }
	    }

        #endregion
    }
}
