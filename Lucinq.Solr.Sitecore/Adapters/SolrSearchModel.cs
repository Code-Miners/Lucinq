using System.Text;
using SolrNet.Commands.Parameters;

namespace Lucinq.Solr.Sitecore.Adapters
{
    public class SolrSearchModel
    {

        public StringBuilder QueryBuilder { get; set; }

        public StringBuilder FilterBuilder { get; set; }

        public QueryOptions QueryOptions { get; }

        public bool IncludeTotalNumberOfSearchResults { get; set; }

        public SolrSearchModel()
        {
            QueryBuilder = new StringBuilder();
            FilterBuilder = new StringBuilder();
            QueryOptions = new QueryOptions();
        }
    }
}
