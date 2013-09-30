using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
    public interface IQueryBuilder : IQueryBuilderGroup, IQueryBuilderIndividual, IHierarchicalQueryGroup
    {
	    void Filter(Filter filter);

	    Filter CurrentFilter { get; }
    }
}
