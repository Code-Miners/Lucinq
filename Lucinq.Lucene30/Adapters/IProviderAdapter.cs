using Lucinq.Core.Querying;

namespace Lucinq.Lucene30.Adapters
{
    public interface IProviderAdapter<T>
    {
        T Adapt(LucinqQueryModel model);
    }
}