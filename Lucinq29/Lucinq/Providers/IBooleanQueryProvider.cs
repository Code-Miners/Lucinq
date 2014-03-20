namespace Lucinq.Providers
{
    using Lucinq.Enums;

    public interface IBooleanQueryProvider<out TQuery> : IBooleanQueryProvider, IQueryProvider<TQuery>
    {
    }

    public interface IBooleanQueryProvider
    {
        void Add<T>(T query, Matches occur);
    }
}
