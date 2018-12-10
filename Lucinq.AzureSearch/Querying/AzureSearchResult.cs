using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Lucinq.AzureSearch.Querying
{
	public class AzureSearchResult : IAzureSearchResult
    {
        #region [ Fields ]

	    private int totalHits;
	    private bool searchExecuted;
	    private DocumentSearchResult topDocs;

        protected string IndexName { get; }

        protected AzureSearchDetails AzureSearchDetails { get; }

        protected AzureSearchModel Model { get; }

        #endregion

        #region [ Constructors ]

        public AzureSearchResult(AzureSearchModel model, AzureSearchDetails azureSearchDetails, string indexName)
        {
            Model = model;
            AzureSearchDetails = azureSearchDetails;
            IndexName = indexName;
        }

		#endregion

		#region [ Properties ]

		public int TotalHits
		{
		    get
		    {
                ExecuteSearch(null, null);
		        return totalHits;
		    }
		}

		public long ElapsedTimeMs { get; set; }

        #endregion

		#region [ Methods ]

		public virtual IList<SearchResult> GetTopItems()
		{
		    ExecuteSearch(null, 30);

		    return topDocs.Results;
		}

        /// <summary>
        /// Gets a range of items on a zero based index
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public virtual IList<SearchResult> GetRange(int start, int end)
        {
            if (start < 0)
            {
                start = 0;
            }

            int take = (end - start) + 1;

            ExecuteSearch(start, take);

            return topDocs.Results;
        }

        private void ExecuteSearch(int? skip, int? take)
	    {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

	        Model.SearchParameters.Top = take;
	        Model.SearchParameters.Skip = skip;
	        Model.SearchParameters.IncludeTotalResultCount = true;


	        using (ISearchServiceClient serviceClient = new SearchServiceClient(AzureSearchDetails.SearchServiceName, new SearchCredentials(AzureSearchDetails.AdminApiKey)))
	        {
	            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient(IndexName);
	            topDocs = indexClient.Documents.Search(Model.QueryBuilder.ToString(), Model.SearchParameters);
	            totalHits = (int) (topDocs?.Count);
	        }

	        stopwatch.Stop();
	        ElapsedTimeMs = stopwatch.ElapsedMilliseconds;
	    }

	    #endregion

        #region [ IEnumerable Methods ]

        public IEnumerator<SearchResult> GetEnumerator()
        {
            return GetTopItems().GetEnumerator();
	    }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return GetEnumerator();
        }

        #endregion
    }
}
