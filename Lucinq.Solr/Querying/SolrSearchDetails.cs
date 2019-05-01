namespace Lucinq.Solr.Querying
{
    public class SolrSearchDetails
    {
        public SolrSearchDetails(string searchServiceName, string adminApiKey)
        {
            SearchServiceName = searchServiceName;
            AdminApiKey = adminApiKey;
        }

        public string AdminApiKey { get; }

        public string SearchServiceName { get; }
    }
}
