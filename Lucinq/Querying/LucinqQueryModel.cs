namespace Lucinq.Core.Querying
{
    public class LucinqQueryModel
    {
        public LucinqQuery Query { get; }

        public LucinqSort Sort { get; }

        public LucinqFilter Filter { get; }

        public LucinqQueryModel(LucinqQuery query, LucinqSort sort, LucinqFilter filter)
        {
            Query = query;
            Sort = sort;
            Filter = filter;
        }
    }
}
