using System.Collections;
using System.Collections.Generic;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
    public class ItemResult<T> : IItemResult<T>
    {
        public ItemResult(List<T> items, int totalHits)
        {
            Items = items;
            TotalHits = totalHits;
        }

        public int TotalHits { get; private set; }

        public long ElapsedTimeMs { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<T> Items { get; private set; }
    }
}
