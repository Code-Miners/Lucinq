using Lucinq.Core.Querying;

namespace Lucinq.Core.Interfaces
{
    public interface IQueryBuilder : IQueryBuilderGroup, IQueryBuilderIndividual, IHierarchicalQueryGroup
    {
	    void Filter(LucinqFilter filter);

        bool CaseSensitive { get; set; }
    }
}
