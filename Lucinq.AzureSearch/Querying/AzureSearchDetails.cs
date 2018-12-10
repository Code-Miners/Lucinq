namespace Lucinq.AzureSearch.Querying
{
    public class AzureSearchDetails
    {
        public AzureSearchDetails(string searchServiceName, string adminApiKey)
        {
            SearchServiceName = searchServiceName;
            AdminApiKey = adminApiKey;
        }

        public string AdminApiKey { get; }

        public string SearchServiceName { get; }
    }
}
