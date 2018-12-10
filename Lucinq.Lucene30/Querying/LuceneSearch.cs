using System;
using System.Diagnostics;
using System.IO;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Core.Interfaces;
using Lucinq.Core.Querying;
using Lucinq.Core.Results;
using Lucinq.Lucene30.Adapters;
using Directory = Lucene.Net.Store.Directory;

namespace Lucinq.Lucene30.Querying
{
    public class LuceneSearch : ILuceneSearch<ILuceneSearchResult>, IIndexSearcherAccessor
    {
        protected LuceneAdapter Adapter { get; }

        #region [ Fields ]

        private readonly string indexPath;

        private readonly IIndexSearcherProvider indexSearcherProvider;

        #endregion

        #region [ Constructors ]

        public LuceneSearch(LuceneAdapter adapter)
        {
            Adapter = adapter;
        }

        public LuceneSearch(String indexPath, LuceneAdapter adapter) : this(adapter)
        {
            this.indexPath = indexPath;
        }

        public LuceneSearch(String indexPath) : this(indexPath, new LuceneAdapter())
        {
        }

        public LuceneSearch(Directory indexDirectory, LuceneAdapter adapter) : this(adapter)
        {
            indexSearcherProvider = new DirectorySearcherProvider(indexDirectory, false);
        }

        public LuceneSearch(Directory indexDirectory) : this(indexDirectory, new LuceneAdapter())
        {
        }

        public LuceneSearch(IIndexSearcherProvider indexSearcherProvider, LuceneAdapter adapter) : this(adapter)
        {
            this.indexSearcherProvider = indexSearcherProvider;
        }

        public LuceneSearch(IIndexSearcherProvider indexSearcherProvider) : this(indexSearcherProvider, new LuceneAdapter())
        {
        }

        #endregion


        #region [ Methods ]


        public virtual void Collect(IQueryBuilder queryBuilder, Collector customCollector)
        {
            LucinqQueryModel model = queryBuilder.Build();
            Collect(model, customCollector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="customCollector"></param>
        /// <param name="filter"></param>
        public virtual void Collect(LucinqQueryModel model, Collector customCollector)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var collectorSearcherProvider = GetIndexSearcherProvider())
            {
                LuceneModel nativeModel = Adapter.Adapt(model);
                if (model.Filter == null)
                {
                    collectorSearcherProvider.IndexSearcher.Search(nativeModel.Query, customCollector);
                }
                else
                {
                    // todo: NM - Collection needs some thought
                    // collectorSearcherProvider.IndexSearcher.Search(Adapter.Adapt(model.Query), FilterAdapter.Adapt(filter), customCollector);
                }

                stopwatch.Stop();
            }
        }

        public virtual ILuceneSearchResult Execute(LucinqQueryModel lucinqQueryModel, int noOfResults = Int32.MaxValue - 1)
        {
            LuceneModel nativeModel = Adapter.Adapt(lucinqQueryModel);
            return new LuceneSearchResult(this, nativeModel);
        }

        public virtual ILuceneSearchResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1)
        {
            LucinqQueryModel lucinqQueryModel = queryBuilder.Build();
            return Execute(lucinqQueryModel, noOfResults);
        }

        public virtual TItemResult Execute<TItemResult, T>(IQueryBuilder queryBuilder, Func<ISearchResult<Document>, TItemResult> creator, int noOfResults = Int32.MaxValue - 1) where TItemResult : ItemSearchResult<Document, T>
        {
            LucinqQueryModel lucinqQueryModel = queryBuilder.Build();
            ILuceneSearchResult luceneSearchResult = Execute(lucinqQueryModel, noOfResults );
            return creator(luceneSearchResult);
        }

        public virtual IIndexSearcherProvider GetIndexSearcherProvider()
        {
            return indexSearcherProvider ?? new DirectorySearcherProvider(FSDirectory.Open(new DirectoryInfo(indexPath)));
        }

        #endregion
    }

    public abstract class LuceneItemSearch<TItemResult, T> : LuceneSearch where TItemResult : ItemSearchResult<Document, T>
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

        protected abstract TItemResult GetItemCreator(ISearchResult<Document> searchResult);
    }
}
