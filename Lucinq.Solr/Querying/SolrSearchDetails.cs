namespace Lucinq.Solr.Querying
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
