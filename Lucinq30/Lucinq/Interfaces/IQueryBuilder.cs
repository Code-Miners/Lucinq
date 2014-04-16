using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
    public interface IQueryBuilder : IQueryBuilderGroup, IQueryBuilderIndividual, IHierarchicalQueryGroup, IQueryBuilderApiSpecific
    {
	    void Filter(Filter filter);

	    Filter CurrentFilter { get; }
    }
}
