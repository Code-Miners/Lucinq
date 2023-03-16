using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lucinq.Core.Adapters;
using Lucinq.Core.Interfaces;
using Lucinq.Core.Querying;
using Lucinq.Core.Results;
using Lucinq.Solr.Sitecore.Adapters;

namespace Lucinq.Solr.Sitecore.Querying
{
    public class SolrSearch : ISolrSearch<ISolrSearchResult>
    {
        public IProviderAdapter<SolrSearchModel> Adapter { get; }

        protected SolrSearchDetails SolrSearchDetails { get; }

        protected string IndexName { get; }

        #region [ Constructors ]

        public SolrSearch(IProviderAdapter<SolrSearchModel> adapter, SolrSearchDetails solrSearchDetails, string indexName)
        {
            Adapter = adapter;
            SolrSearchDetails = solrSearchDetails;
            IndexName = indexName;
        }

        public SolrSearch(SolrSearchDetails solrSearchDetails, string indexName) : this(new SolrSearchAdapter(), solrSearchDetails, indexName)
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
            return new SolrSearchResult(nativeModel, SolrSearchDetails, IndexName);
        }

        public virtual ISolrSearchResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1)
        {
            return Execute(queryBuilder.Build(), noOfResults);
        }

        public virtual TItemResult Execute<TItemResult, T>(IQueryBuilder queryBuilder, Func<ISolrSearchResult, TItemResult> creator, int noOfResults = Int32.MaxValue - 1) where TItemResult : ItemSearchResult<Dictionary<string, object>, T>
        {
            ISolrSearchResult searchResult = Execute(queryBuilder.Build(), noOfResults);
            return creator(searchResult);
        }

        #endregion
    }
}
