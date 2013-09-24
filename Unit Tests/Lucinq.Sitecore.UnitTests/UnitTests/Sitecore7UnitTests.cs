using System;
using Lucinq.Interfaces;
using Lucinq.Querying;
using Lucinq.SitecoreIntegration.Extensions;
using Lucinq.SitecoreIntegration.Extensions.Sitecore7;
using NUnit.Framework;

namespace Lucinq.Sitecore.UnitTests.UnitTests
{
    [TestFixture]
    public class Sitecore7UnitTests
    {
        #region [ Field Name Tests ]

        [Test]
        public void GetIndexFieldName()
        {
            TestSitecore7Class testClass = new TestSitecore7Class();
            string fieldName = FieldExtensions.GetFieldName<TestSitecore7Class>(t => t.TemplateId);
            Assert.AreEqual("_template", fieldName);
            Assert.AreEqual("_template", testClass.GetFieldName(t => t.TemplateId));
        }

        [Test]
        public void GetIndexFieldNameWhereNoneSpecified()
        {
            TestSitecore7Class testClass = new TestSitecore7Class();
            string fieldName = FieldExtensions.GetFieldName<TestSitecore7Class>(t => t.UnspecifiedField);
            Assert.AreEqual("unspecifiedfield", fieldName);
            Assert.AreEqual("unspecifiedfield", testClass.GetFieldName(t => t.UnspecifiedField));
        }

        #endregion

        #region [ Field Extensions ]

        [Test]
        public void Term()
        {
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Term<TestSitecore7Class>(t => t.Content, "value");

            IQueryBuilder queryBuilder2 = new QueryBuilder();
            queryBuilder2.Term("_content", "value");

            Assert.AreEqual(queryBuilder2.Build().ToString(), queryBuilder.Build().ToString());
        }

        [Test]
        public void Phrase()
        {
            throw new NotImplementedException("Needs writing");
        }

        [Test]
        public void WildCard()
        {
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.WildCard<TestSitecore7Class>(t => t.Content, "value*");

            IQueryBuilder queryBuilder2 = new QueryBuilder();
            queryBuilder2.WildCard("_content", "value*");

            Assert.AreEqual(queryBuilder2.Build().ToString(), queryBuilder.Build().ToString());
        }

        #endregion
    }
}
