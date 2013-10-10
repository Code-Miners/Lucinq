using System;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Globalization;

namespace Lucinq.GlassMapper.Sitecore.UnitTests.Objects
{
    [SitecoreType(TemplateId = "{B6E1D6BA-EE54-4E71-8003-E6A1AF7119EB}", AutoMap = true)]
	public class GlassAttributeTestClass
	{
		[SitecoreField]
		public string Text { get; set; }
		
		[SitecoreId]
		public Guid Id { get; set; }

		[SitecoreInfo(SitecoreInfoType.ContentPath)]
		public string ContentPath { get; set; }

		[SitecoreInfo(SitecoreInfoType.DisplayName)]
		public string SitecoreDisplayName { get; set; }

		[SitecoreInfo(SitecoreInfoType.Language)]
		public Language Language { get; set; }

		[SitecoreField("Different Name")]
		public string DifferentName { get; set; }
	}

    [SitecoreType(AutoMap = true)]
    public class PageContent
    {
        [SitecoreId]
        public Guid Id { get; set; }

        [SitecoreField("Title")]
        public string PageTitle { get; set; }

        [SitecoreField("Subtitle")]
        public string PageSubtitle { get; set; }
    }

	public class GlassMappedTestClass
	{
		public string Text { get; set; }
	}
}
