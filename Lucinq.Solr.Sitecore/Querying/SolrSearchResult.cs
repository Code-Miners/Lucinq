using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CommonServiceLocator;
using Lucinq.Solr.Sitecore.Adapters;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
using SolrNet;
using SolrNet.Impl;

namespace Lucinq.Solr.Sitecore.Querying
{
    public class SolrSearchResult : ISolrSearchResult
    {
        #region [ Fields ]

        private int totalHits;
        private bool searchExecuted;
        private IList<Dictionary<string, object>> topDocs;

        protected string IndexName { get; }

        protected SolrSearchDetails SolrSearchDetails { get; }

        protected SolrSearchModel Model { get; }

        #endregion

        #region [ Constructors ]

        public SolrSearchResult(SolrSearchModel model, SolrSearchDetails solrSearchDetails, string indexName)
        {
            Model = model;
            SolrSearchDetails = solrSearchDetails;
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

        public virtual IList<Dictionary<string, object>> GetTopItems()
        {
            ExecuteSearch(null, 30);

            return topDocs;
        }

        /// <summary>
        /// Gets a range of items on a zero based index
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public virtual IList<Dictionary<string, object>> GetRange(int start, int end)
        {
            if (start < 0)
            {
                start = 0;
            }

            int take = (end - start) + 1;

            ExecuteSearch(start, take);

            return topDocs;
        }

        private void ExecuteSearch(int? skip, int? take)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (skip == null)
            {
                skip = 0;
            }

            Model.QueryOptions.Start = (int)skip;
            Model.QueryOptions.Rows = take;

            Model.IncludeTotalNumberOfSearchResults = true;
            if (Model.QueryBuilder.ToString() == String.Empty)
            {
                Model.QueryBuilder.Append("*:*");
            }

            var solrQuery = new SolrQuery(Model.QueryBuilder.ToString());
            Model.QueryOptions.FilterQueries.Add(new SolrQuery(Model.FilterBuilder.ToString()));
            Model.QueryOptions.ExtraParams = new List<KeyValuePair<string, string>>
                { /*new KeyValuePair<string, string>("wt", "xml")*/ };


            using (IProviderSearchContext context = ContentSearchManager.GetIndex(IndexName).CreateSearchContext())
            {
                var results = context.Query<Dictionary<string, object>>(solrQuery, Model.QueryOptions);
                topDocs = results;
                totalHits = topDocs.Count;
            }

            stopwatch.Stop();
            ElapsedTimeMs = stopwatch.ElapsedMilliseconds;
        }

    
        #endregion

        #region [ IEnumerable Methods ]

        public IEnumerator<Dictionary<string, object>> GetEnumerator()
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
