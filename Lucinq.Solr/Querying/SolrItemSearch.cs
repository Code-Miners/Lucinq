﻿namespace Lucinq.Solr.Querying
{
    using System;
    using System.Collections.Generic;
    using Adapters;
    using Core.Adapters;
    using Core.Interfaces;
    using Core.Results;

    public abstract class SolrItemSearch<TItemResult, T> : SolrSearch where TItemResult : ItemSearchResult<Dictionary<string, object>, T>
    {
        public SolrItemSearch(IProviderAdapter<SolrSearchModel> adapter, SolrSearchDetails azureSearchDetails, string indexName) : base(adapter, azureSearchDetails, indexName)
        {
        }

        public SolrItemSearch(SolrSearchDetails azureSearchDetails, string indexName) : this(new SolrSearchAdapter(), azureSearchDetails, indexName)
        {
        }

        public new virtual TItemResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1)
        {
            return Execute<TItemResult, T>(queryBuilder, GetItemCreator, noOfResults);
        }

        protected abstract TItemResult GetItemCreator(ISearchResult<Dictionary<string, object>> searchResult);
    }
}
