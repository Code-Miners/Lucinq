using Lucinq.SitecoreIntegration.Querying;
using Lucinq.SitecoreIntegration.Querying.Interfaces;
using NUnit.Framework;
using Lucinq.Interfaces;
using Lucinq.Querying;
using Lucinq.SitecoreIntegration.Constants;
using Lucinq.SitecoreIntegration.Extensions;
using Sitecore.Globalization;

namespace Lucinq.SitecoreIntegration.UnitTests.UnitTests
{
    [TestFixture]
    public class EquivalencyTests
    {
        [Test]
        public void IdEquivalent()
        {
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Term(SitecoreFields.Id, SitecoreIds.HomeItemId.ToLuceneId()));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.Id(SitecoreIds.HomeItemId));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        /// <summary>
        /// The ancestor equivalent.
        /// </summary>
        [Test]
        public void AncestorEquivalent()
        {
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Term(SitecoreFields.Path, SitecoreIds.HomeItemId.ToLuceneId()));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.DescendantOf(SitecoreIds.HomeItemId));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        [Test]
        public void BaseTemplateEquivalent()
        {
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Term(SitecoreFields.TemplatePath, SitecoreIds.HomeItemId.ToLuceneId()));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.TemplateDescendsFrom(SitecoreIds.HomeItemId));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        [Test]
        public void TemplateEquivalent()
        {
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Term(SitecoreFields.TemplateId, SitecoreIds.HomeItemId.ToLuceneId()));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.TemplateId(SitecoreIds.HomeItemId));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        [Test]
        public void TemplateIdsEquivalent()
        {
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(
                    x => x.And(
                        y => y.Term(SitecoreFields.TemplateId, SitecoreIds.HomeItemId.ToLuceneId()),
                        y => y.Term(SitecoreFields.TemplateId, SitecoreIds.HomeItemId.ToLuceneId())));

            var templateIds = new[] { SitecoreIds.HomeItemId, SitecoreIds.HomeItemId };
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
            ISitecoreQueryBuilder expectedQueryBuilder =
                new SitecoreQueryBuilder(x => x.Term(SitecoreFields.Parent, SitecoreIds.HomeItemId.ToLuceneId()));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.ChildOf(SitecoreIds.HomeItemId));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }

        [Test]
        public void NameEquivalent()
        {
            IQueryBuilder expectedQueryBuilder =
                new QueryBuilder(x => x.Term(SitecoreFields.Name, "value"));

            ISitecoreQueryBuilder actualQueryBuilder = new SitecoreQueryBuilder(x => x.Name("value"));
            Assert.AreEqual(expectedQueryBuilder.Build().ToString(), actualQueryBuilder.Build().ToString());
        }
    }
}
