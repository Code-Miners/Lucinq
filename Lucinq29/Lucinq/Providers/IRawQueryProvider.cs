namespace Lucinq.Providers
{
    public interface IRawQueryProvider : IQueryProvider
    {
        string QueryText { get; set; }
    }
}
