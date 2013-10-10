using System;
using Glass.Mapper;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.CastleWindsor;
using Lucinq.GlassMapper.Sitecore.UnitTests.App_Start;
using Lucinq.GlassMapper.Sitecore.UnitTests.Objects;
using Lucinq.GlassMapper.SitecoreIntegration;
using Lucinq.GlassMapper.SitecoreIntegration.Extensions;
using Lucinq.GlassMapper.SitecoreIntegration.Interfaces;
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
            search = new LuceneSearch(IndexPath);
            service = new SitecoreService("web");
		}

		private const string IndexPath = @"C:\Work\BoltonWanderers\Source\Websites\Sitecore\App_Data\SitecoreData\indexes\lucinq_web_index";

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
            queryBuilder.Field<PageContent>(t => t.PageTitle, "*new*");
            IGlassSearchResult sitecoreSearchResult = new GlassSearchResult(service, search.Execute(queryBuilder));
            IGlassItemResult<PageContent> result = sitecoreSearchResult.GetPagedItems<PageContent>(0, 20);
            foreach (PageContent item in result)
            {
                Console.WriteLine("Page Title: " + item.PageTitle);
                Console.WriteLine("Page Subtitle: " + item.PageSubtitle);
                Console.WriteLine("Id: " + item.Id);
            }
	    }

        #endregion
    }
}
