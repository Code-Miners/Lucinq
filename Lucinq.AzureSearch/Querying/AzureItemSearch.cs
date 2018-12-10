using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucinq.AzureSearch.Adapters;
using Lucinq.Core.Adapters;
using Lucinq.Core.Interfaces;
using Lucinq.Core.Results;
using Microsoft.Azure.Search.Models;

namespace Lucinq.AzureSearch.Querying
{
    public abstract class AzureItemSearch<TItemResult, T> : AzureSearch where TItemResult : ItemSearchResult<SearchResult, T>
    {
        public AzureItemSearch(IProviderAdapter<AzureSearchModel> adapter, AzureSearchDetails azureSearchDetails, string indexName) : base(adapter, azureSearchDetails, indexName)
        {
        }

        public AzureItemSearch(AzureSearchDetails azureSearchDetails, string indexName) : this(new AzureSearchAdapter(), azureSearchDetails, indexName)
        {
        }

        public new virtual TItemResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1)
        {
            return Execute<TItemResult, T>(queryBuilder, GetItemCreator, noOfResults);
        }

        protected abstract TItemResult GetItemCreator(ISearchResult<SearchResult> searchResult);
    }
}
