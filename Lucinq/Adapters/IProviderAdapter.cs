using Lucinq.Core.Querying;

namespace Lucinq.Core.Adapters
{
    public interface IProviderAdapter<T>
    {
        T Adapt(LucinqQueryModel model);
    }
}