namespace Lucinq.Solr.Sitecore.Querying
{
    public class SolrSearchDetails
    {
        public SolrSearchDetails(string searchServiceName)
        {
            SearchServiceName = searchServiceName;
        }


        public string SearchServiceName { get; }
    }
}
