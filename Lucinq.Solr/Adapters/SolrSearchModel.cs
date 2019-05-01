namespace Lucinq.Solr.Adapters
{
    using System.Text;
    using Microsoft.Azure.Search.Models;

    public class SolrSearchModel
    {
        public SearchParameters SearchParameters { get; }

        public StringBuilder QueryBuilder { get; set; }

        public StringBuilder FilterBuilder { get; set; }

        public SolrSearchModel()
        {
            QueryBuilder = new StringBuilder();
            FilterBuilder = new StringBuilder();
            SearchParameters = new SearchParameters();
        }
    }
}
