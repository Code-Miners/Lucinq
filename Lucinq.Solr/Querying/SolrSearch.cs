namespace Lucinq.Solr.Querying
{
    using System;
    using System.Diagnostics;
    using Adapters;
    using Core.Adapters;
    using Core.Interfaces;
    using Core.Querying;
    using Core.Results;
    using Microsoft.Azure.Search.Models;

    public class SolrSearch : ISolrSearch<ISolrSearchResult>
    {
        public IProviderAdapter<SolrSearchModel> Adapter { get; }

        protected SolrSearchDetails AzureSearchDetails { get; }

        protected string IndexName { get; }

        #region [ Constructors ]

        public SolrSearch(IProviderAdapter<SolrSearchModel> adapter, SolrSearchDetails azureSearchDetails, string indexName)
        {
            Adapter = adapter;
            AzureSearchDetails = azureSearchDetails;
            IndexName = indexName;
        }

        public SolrSearch(SolrSearchDetails azureSearchDetails, string indexName) : this(new SolrSearchAdapter(), azureSearchDetails, indexName)
        {
        }

        #endregion

        #region [ Methods ]


        public virtual void Collect(IQueryBuilder queryBuilder)
        {
            Collect(queryBuilder.Build());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lucinqModel"></param>
        public virtual void Collect(LucinqQueryModel lucinqModel)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            /*using (var collectorSearcherProvider = GetIndexSearcherProvider())
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
            }*/
        }

        public virtual ISolrSearchResult Execute(LucinqQueryModel lucinqModel, int noOfResults = Int32.MaxValue - 1)
        {
            var nativeModel = Adapter.Adapt(lucinqModel);
            return new AzureSearchResult(nativeModel, AzureSearchDetails, IndexName);
        }

        public virtual ISolrSearchResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1)
        {
            return Execute(queryBuilder.Build(), noOfResults);
        }

        public virtual TItemResult Execute<TItemResult, T>(IQueryBuilder queryBuilder, Func<ISolrSearchResult, TItemResult> creator, int noOfResults = Int32.MaxValue - 1) where TItemResult : ItemSearchResult<SearchResult, T>
        {
            ISolrSearchResult searchResult = Execute(queryBuilder.Build(), noOfResults);
            return creator(searchResult);
        }

        #endregion
    }
}
