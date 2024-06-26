﻿using System;
using AutoMapper;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Lucinq.Core.Interfaces;
using Lucinq.Core.Querying;
using Lucinq.Core.Results;
using Lucinq.Lucene30.Querying;
using Lucinq.Lucene30.UnitTests.IntegrationTests;
using NUnit.Framework;

namespace Lucinq.Lucene30.UnitTests.UnitTests
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
            var results = luceneSearch.Execute<DelegateItemSearchResult<Document, NewsArticle>, NewsArticle>(queryBuilder, x => new DelegateItemSearchResult<Document, NewsArticle>(x, GetNewsArticleFromDocument));
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
            DelegateSearchResultFactory<Document, NewsArticle> articleFactory = new DelegateSearchResultFactory<Document, NewsArticle>(GetNewsArticleFromDocument);
            var results = luceneSearch.Execute<DelegateItemSearchResult<Document, NewsArticle>, NewsArticle>(queryBuilder, articleFactory.GetItemResult);
            var newsArticles = results.GetRange(0, 2);
            Assert.Greater(results.TotalHits, newsArticles.Items.Count);
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
            var newsArticles = results.GetRange(0, 2);
            Assert.Greater(results.TotalHits, newsArticles.Items.Count);
            foreach (var newsArticle in newsArticles)
            {
                Console.WriteLine(newsArticle.Title);
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
                Console.WriteLine(newsArticle.Title);
                Assert.IsTrue(newsArticle.Title.IndexOf("africa", StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        #region [ Item Result Tests ]

        [Test]
        public void GetItemsFromNewsArticleSearch()
        {
            NewsArticleSearch newsArticleSearch = new NewsArticleSearch(new DirectorySearcherProvider(IndexDirectory, false));

            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Term(BBCFields.Title, "africa");
            var results = newsArticleSearch.Execute(queryBuilder);
            var newsArticles = results.GetRange(0, 2);
            Assert.Greater(results.TotalHits, newsArticles.Items.Count);
            Assert.Greater(newsArticles.TotalHits, newsArticles.Items.Count);
            Assert.Greater(newsArticles.ElapsedTimeMs, 0);
            Console.WriteLine("Lucene Search Took {0}ms", results.ElapsedTimeMs);
            Console.WriteLine("News Population Took {0}ms", newsArticles.ElapsedTimeMs);
            foreach (var newsArticle in newsArticles)
            {
                Console.WriteLine(newsArticle.Title);
                Assert.IsTrue(newsArticle.Title.IndexOf("africa", StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        #endregion

        #region [ Helpers ]

        public static NewsArticle GetNewsArticleFromDocument(Document doc)
        {
            return Mapper.Map<Document, NewsArticle>(doc);
        }

        public class NewsArticleItemResult : ItemSearchResult<Document, NewsArticle>
        {
            public NewsArticleItemResult(ISearchResult<Document> luceneSearchResult) : base(luceneSearchResult)
            {
            }

            public override NewsArticle GetItem(Document document)
            {
                return GetNewsArticleFromDocument(document);
            }
        }

        public class NewsArticleSearch : LuceneItemSearch<NewsArticleItemResult, NewsArticle>
        {
            public NewsArticleSearch(string indexPath) : base(indexPath)
            {
            }

            public NewsArticleSearch(Directory indexDirectory) : base(indexDirectory)
            {
            }

            public NewsArticleSearch(IIndexSearcherProvider indexSearcherProvider) : base(indexSearcherProvider)
            {
            }

            protected override NewsArticleItemResult GetItemCreator(ISearchResult<Document> searchResult)
            {
                return new NewsArticleItemResult(searchResult);
            }
        }

        #endregion
    }
}
