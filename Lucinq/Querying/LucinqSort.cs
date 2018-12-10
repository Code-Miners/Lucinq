namespace Lucinq.Core.Querying
{
    public class LucinqSort
    {
        public LucinqSortField[] SortFields { get; }

        public LucinqSort(LucinqSortField[] sortFields)
        {
            SortFields = sortFields;
        }
    }
}
