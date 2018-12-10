using System.Text;
using Microsoft.Azure.Search.Models;

namespace Lucinq.AzureSearch.Querying
{
    public class AzureSearchModel
    {
        public SearchParameters SearchParameters { get; }

        public StringBuilder QueryBuilder { get; set; }

        public StringBuilder FilterBuilder { get; set; }

        public AzureSearchModel()
        {
            QueryBuilder = new StringBuilder();
            FilterBuilder = new StringBuilder();
            SearchParameters = new SearchParameters();
        }
    }
}
