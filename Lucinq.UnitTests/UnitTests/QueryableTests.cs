using System;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucinq.Interfaces;
using Lucinq.Querying;
using Lucinq.UnitTests.IntegrationTests;
using NUnit.Framework;

namespace Lucinq.UnitTests.UnitTests
{
    [TestFixture]
    public class QueryableTests : IntegrationTestBase
    {
        #region [ Fields ]

        private static readonly LuceneSearch memorySearch = new LuceneSearch(GeneralConstants.Paths.BBCIndex, true);
        private static LuceneSearch filesystemSearch = new LuceneSearch(GeneralConstants.Paths.BBCIndex);
        private static readonly LuceneSearch[] searches = { filesystemSearch, memorySearch };

        #endregion

        [Test]
        public void CaseInsensitiveMandatoryTerm()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            Term term = new Term("_name", "value");
            TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Occur.MUST);
            string queryString = originalQuery.ToString();


            // LuceneQueryable luceneQueryable = new LuceneQueryable();
            //var items = luceneQueryable.Where(x => x.Term("_name", "Value"));
            //Query replacementQuery = luceneQueryable.Build();
            //string newQueryString = replacementQuery.ToString();

            //Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        [Test, TestCaseSource("searches")]
        public void BoostedCaseInsensitiveMandatoryTerm(LuceneSearch luceneSearch)
        {
            BooleanQuery originalQuery = new BooleanQuery();
            Term term = new Term("_name", "value");
            TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Occur.MUST);
            termQuery.Boost = 10;
            string queryString = originalQuery.ToString();

            IQueryable<Document> luceneQueryable = luceneSearch.GetQueryable();
            var items = luceneQueryable.Where(x => x.Term("_name", "Value"));
            //Query replacementQuery = luceneQueryable.Build();
            // string newQueryString = replacementQuery.ToString();

            // Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

    }
}
