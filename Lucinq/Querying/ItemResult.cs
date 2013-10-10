using System.Collections;
using System.Collections.Generic;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
	public class ItemResult<T> : IItemResult<T>
	{
		#region [ Constructors ]

		public ItemResult(List<T> items, int totalHits)
		{
			Items = items;
			TotalHits = totalHits;
		} 

		#endregion

		#region [ Properties ]

		public int TotalHits { get; private set; }

		public long ElapsedTimeMs { get; set; }

		public List<T> Items { get; private set; }

		#endregion

        #region [ Enumerable Methods ]

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return GetEnumerator();
        }

        #endregion
    }
}
