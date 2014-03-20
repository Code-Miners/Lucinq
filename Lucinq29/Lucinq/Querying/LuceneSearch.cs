using System;
using System.Diagnostics;
using System.IO;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Interfaces;
using Directory = Lucene.Net.Store.Directory;

namespace Lucinq.Querying
{
    public class LuceneSearch : ILuceneSearch<ILuceneSearchResult>, IIndexSearcherAccessor
    {
        #region [ Fields ]

        private readonly string indexPath;

        private readonly IIndexSearcherProvider indexSearcherProvider;

        #endregion

        #region [ Constructors ]

        public LuceneSearch(String indexPath)
        {
            this.indexPath = indexPath;
        }

        public LuceneSearch(Directory indexDirectory)
        {
            indexSearcherProvider = new DirectorySearcherProvider(indexDirectory, false);
        }

        public LuceneSearch(IIndexSearcherProvider indexSearcherProvider)
        {
            this.indexSearcherProvider = indexSearcherProvider;
        }

        #endregion

        #region [ Methods ]


        public virtual void Collect(IQueryBuilder queryBuilder, Collector customCollector, Filter filter = null)
        {
            Collect(queryBuilder.Build(), customCollector, filter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="customCollector"></param>
        /// <param name="filter"></param>
        public virtual void Collect(Query query, Collector customCollector, Filter filter = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var collectorSearcherProvider = GetIndexSearcherProvider())
            {
                if (filter == null)
                {
                    collectorSearcherProvider.IndexSearcher.Search(query, customCollector);
                }
                else
                {
                    collectorSearcherProvider.IndexSearcher.Search(query, filter, customCollector);
                }

                stopwatch.Stop();
            }
        }

        public virtual ILuceneSearchResult Execute(Query query, int noOfResults = Int32.MaxValue - 1, Sort sort = null, Filter filter = null)
        {
            return new LuceneSearchResult(this, query, sort, filter);
        }

        public virtual ILuceneSearchResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1)
        {
            return Execute(queryBuilder.Build(), noOfResults, queryBuilder.CurrentSort, queryBuilder.CurrentFilter);
        }

        public virtual TItemResult Execute<TItemResult, T>(IQueryBuilder queryBuilder, Func<ILuceneSearchResult, TItemResult> creator, int noOfResults = Int32.MaxValue - 1) where TItemResult : ItemSearchResult<T>
        {
            ILuceneSearchResult luceneSearchResult = Execute(queryBuilder.Build(), noOfResults,
                queryBuilder.CurrentSort, queryBuilder.CurrentFilter);
            return creator(luceneSearchResult);
        }

        public virtual IIndexSearcherProvider GetIndexSearcherProvider()
        {
            return indexSearcherProvider ?? new DirectorySearcherProvider(FSDirectory.Open(new DirectoryInfo(indexPath)));
        }

        #endregion
    }

    public abstract class LuceneItemSearch<TItemResult, T> : LuceneSearch where TItemResult : ItemSearchResult<T>
    {
        protected LuceneItemSearch(string indexPath) : base(indexPath)
        {
        }

        protected LuceneItemSearch(Directory indexDirectory) : base(indexDirectory)
        {
        }

        protected LuceneItemSearch(IIndexSearcherProvider indexSearcherProvider)
            : base(indexSearcherProvider)
        {
        }

        public new virtual TItemResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1) 
        {
            return Execute<TItemResult, T>(queryBuilder, GetItemCreator, noOfResults);
        }

        protected abstract TItemResult GetItemCreator(ILuceneSearchResult searchResult);
    }
}
