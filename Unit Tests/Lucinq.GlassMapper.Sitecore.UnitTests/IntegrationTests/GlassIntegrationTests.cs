using System;
using Glass.Mapper;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.CastleWindsor;
using Glass.Mapper.Sc.Configuration.Attributes;
using Lucinq.GlassMapper.Sitecore.UnitTests.App_Start;
using Lucinq.GlassMapper.Sitecore.UnitTests.Objects;
using Lucinq.GlassMapper.SitecoreIntegration.Extensions;
using Lucinq.SitecoreIntegration.Constants;
using NUnit.Framework;

namespace Lucinq.GlassMapper.Sitecore.UnitTests.IntegrationTests
{
	[TestFixture]
	public class GlassIntegrationTests
	{

	    private Context _context;

		[TestFixtureSetUp]
		public void Setup()
		{
            var resolver = DependencyResolver.CreateStandardResolver();
            GlassMapperScCustom.CastleConfig(resolver.Container);
            _context = Context.Create(resolver);
            _context.Load(GlassMapperScCustom.GlassLoaders());
            GlassMapperScCustom.PostLoad();
            _context.Load();
		}

		public const string IndexPath = @"c:\tfs\3chillies.visualstudio.com\TwoBirds\Data\indexes\ContentIndex";

		[Test]
		public void GetAttributeField()
		{
			ISitecoreService service = new SitecoreService("web");
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
	}
}
