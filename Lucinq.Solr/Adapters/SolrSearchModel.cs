namespace Lucinq.Solr.Adapters
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using SolrNet;
    using SolrNet.Commands.Parameters;

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
