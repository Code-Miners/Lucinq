using System;
using System.Configuration;
using System.Diagnostics;
using Lucinq.AzureSearch.Adapters;
using Lucinq.Core.Adapters;
using Lucinq.Core.Interfaces;
using Lucinq.Core.Querying;
using Lucinq.Core.Results;
using Microsoft.Azure.Search.Models;

namespace Lucinq.AzureSearch.Querying
{
    public class AzureSearch : IAzureSearch<IAzureSearchResult>
    {
        public IProviderAdapter<AzureSearchModel> Adapter { get; }

        protected AzureSearchDetails AzureSearchDetails { get; }

        protected string IndexName { get; }

        #region [ Constructors ]

        public AzureSearch(IProviderAdapter<AzureSearchModel> adapter, AzureSearchDetails azureSearchDetails, string indexName)
        {
            Adapter = adapter;
            AzureSearchDetails = azureSearchDetails;
            IndexName = indexName;
        }

        public AzureSearch(AzureSearchDetails azureSearchDetails, string indexName) : this(new AzureSearchAdapter(), azureSearchDetails, indexName)
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

        public virtual IAzureSearchResult Execute(LucinqQueryModel lucinqModel, int noOfResults = Int32.MaxValue - 1)
        {
            var nativeModel = Adapter.Adapt(lucinqModel);
            return new AzureSearchResult(nativeModel, AzureSearchDetails, IndexName);
        }

        public virtual IAzureSearchResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1)
        {
            return Execute(queryBuilder.Build(), noOfResults);
        }

        public virtual TItemResult Execute<TItemResult, T>(IQueryBuilder queryBuilder, Func<IAzureSearchResult, TItemResult> creator, int noOfResults = Int32.MaxValue - 1) where TItemResult : ItemSearchResult<SearchResult, T>
        {
            IAzureSearchResult luceneSearchResult = Execute(queryBuilder.Build(), noOfResults);
            return creator(luceneSearchResult);
        }

        #endregion
    }
}
