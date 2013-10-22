using System;
using AutoMapper;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Lucinq.Interfaces;
using Lucinq.Querying;
using Lucinq.UnitTests.IntegrationTests;
using NUnit.Framework;

namespace Lucinq.UnitTests.UnitTests
{
    [TestFixture]
    public class ItemResultTests : BaseTestFixture
    {
        [Test]
        public void GetTopItemsFromDelegateResult()
        {
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearcherProvider(IndexDirectory, false));
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Term(BBCFields.Title, "africa");
            var results = luceneSearch.Execute<DelegateItemResult<NewsArticle>, NewsArticle>(queryBuilder, x => new DelegateItemResult<NewsArticle>(x, GetNewsArticleFromDocument));
            var newsArticles = results.GetTopItems();
            Assert.Greater(results.TotalHits, 0);
            foreach (var newsArticle in newsArticles)
            {
                Console.WriteLine(newsArticle.Title);
                Assert.IsTrue(newsArticle.Title.IndexOf("africa", StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        [Test]
        public void GetTopItemsFromSpecificResult()
        {
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearcherProvider(IndexDirectory, false));
            
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Term(BBCFields.Title, "africa");
            var results = luceneSearch.Execute<NewsArticleItemResult, NewsArticle>(queryBuilder, x => new NewsArticleItemResult(x));
            var newsArticles = results.GetTopItems();
            Assert.Greater(results.TotalHits, 0);
            foreach (var newsArticle in newsArticles)
            {
                Assert.IsTrue(newsArticle.Title.IndexOf("africa", StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        [Test]
        public void GetPagedItemsFromDelegateResult()
        {
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearcherProvider(IndexDirectory, false));
            IQueryBuilder queryBuilder = new QueryBuilder();

            queryBuilder.Term(BBCFields.Title, "africa");
            var results = luceneSearch.Execute<DelegateItemResult<NewsArticle>, NewsArticle>(queryBuilder, x => new DelegateItemResult<NewsArticle>(x, GetNewsArticleFromDocument));
            var newsArticles = results.GetPagedItems(0, 2);
            Assert.Greater(results.TotalHits, newsArticles.Count);
            foreach (var newsArticle in newsArticles)
            {
                Console.WriteLine(newsArticle.Title);
                Assert.IsTrue(newsArticle.Title.IndexOf("africa", StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        [Test]
        public void GetPagedItemsFromSpecificResult()
        {
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearcherProvider(IndexDirectory, false));

            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Term(BBCFields.Title, "africa");
            var results = luceneSearch.Execute<NewsArticleItemResult, NewsArticle>(queryBuilder, x => new NewsArticleItemResult(x));
            var newsArticles = results.GetPagedItems(0, 2);
            Assert.Greater(results.TotalHits, newsArticles.Count);
            foreach (var newsArticle in newsArticles)
            {
                Assert.IsTrue(newsArticle.Title.IndexOf("africa", StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        [Test]
        public void TestEnumerable()
        {
            LuceneSearch luceneSearch = new LuceneSearch(new DirectorySearcherProvider(IndexDirectory, false));

            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Term(BBCFields.Title, "africa");
            var results = luceneSearch.Execute<NewsArticleItemResult, NewsArticle>(queryBuilder, x => new NewsArticleItemResult(x));
            Assert.Greater(results.TotalHits, 0);
            foreach (var newsArticle in results)
            {
                Assert.IsTrue(newsArticle.Title.IndexOf("africa", StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        #region [ Helpers ]

        public static NewsArticle GetNewsArticleFromDocument(Document doc)
        {
            return Mapper.Map<Document, NewsArticle>(doc);
        }

        public class NewsArticleItemResult : ItemResult<NewsArticle>
        {
            public NewsArticleItemResult(ILuceneSearchResult<Document> luceneSearchResult) : base(luceneSearchResult)
            {
            }

            public override NewsArticle GetItem(Document document)
            {
                return GetNewsArticleFromDocument(document);
            }
        }

        #endregion
    }
}
