using Lucinq.SitecoreIntegration.Querying;
using Lucinq.SitecoreIntegration.Querying.Interfaces;
using NUnit.Framework;
using Sitecore.Data;
using Lucinq.Interfaces;
using Lucinq.Querying;
using Lucinq.SitecoreIntegration.Constants;
using Lucinq.SitecoreIntegration.Extensions;
using Sitecore.Globalization;

namespace Lucinq.Sitecore.UnitTests.UnitTests
{
    [TestFixture]
    public class EquivalencyTests
    {
        [Test]
        public void IdEquivalent()
        {
            ID homeItemId = new ID(Constants.HomeItemId);
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Term(SitecoreFields.Id, homeItemId.ToLuceneId()));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.Id(homeItemId));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        /// <summary>
        /// The ancestor equivalent.
        /// </summary>
        [Test]
        public void AncestorEquivalent()
        {
            ID homeItemId = new ID(Constants.HomeItemId);
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Term(SitecoreFields.Path, homeItemId.ToLuceneId()));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.DescendantOf(homeItemId));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        [Test]
        public void BaseTemplateEquivalent()
        {
            ID homeItemId = new ID(Constants.HomeItemId);
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Term(SitecoreFields.TemplatePath, homeItemId.ToLuceneId()));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.BaseTemplateId(homeItemId));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        [Test]
        public void TemplateEquivalent()
        {
            ID homeItemId = new ID(Constants.HomeItemId);
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Term(SitecoreFields.TemplateId, homeItemId.ToLuceneId()));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.TemplateId(homeItemId));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        [Test]
        public void TemplateIdsEquivalent()
        {
            ID homeItemId = new ID(Constants.HomeItemId);
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(
                    x => x.And(
                        y => y.Term(SitecoreFields.TemplateId, homeItemId.ToLuceneId()),
                        y => y.Term(SitecoreFields.TemplateId, homeItemId.ToLuceneId())));

            var templateIds = new[] {homeItemId, homeItemId};
            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.TemplateIds(templateIds));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        [Test]
        public void DatabaseEquivalent()
        {
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Term(SitecoreFields.Database, "web"));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.Database("web"));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        [Test]
        public void LanguageEquivalent()
        {
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Keyword(SitecoreFields.Language, "en-gb"));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.Language(Language.Parse("en-gb")));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        [Test]
        public void ParentEquivalent()
        {
            ID homeItemId = new ID(Constants.HomeItemId);
            ISitecoreQueryBuilder expectedQueryBuilder =
                new SitecoreQueryBuilder(x => x.Term(SitecoreFields.Parent, homeItemId.ToLuceneId()));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.ChildOf(homeItemId));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        [Test]
        public void NameEquivalent()
        {
            ID homeItemId = new ID(Constants.HomeItemId);
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Term(SitecoreFields.Name, "value"));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.Name("value"));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }
    }
}
