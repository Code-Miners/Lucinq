namespace Lucinq.Providers
{
    public interface IQueryProviderService
    {
        IPhraseQueryProvider GetPhraseQueryProvider(int slop);

        IBooleanQueryProvider GetBooleanQueryProvider();

        ITermQueryProvider GetTermQueryProvider(string field, string term, bool? caseSensitive);

        IWildcardQueryProvider GetWildcardQueryProvider();

        IPrefixQueryProvider GetPrefixQueryProvider(string field, string term);

        IRawQueryProvider GetRawQueryProvider(string field, string queryText);

        IFuzzyQueryProvider GetFuzzyQueryProvider(string field, string term);

        void SetBoost(IQueryProvider provider, float? boost);
    }
}
