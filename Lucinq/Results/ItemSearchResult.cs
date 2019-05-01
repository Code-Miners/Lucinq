using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lucinq.Core.Interfaces;
using Lucinq.Core.Querying;

namespace Lucinq.Core.Results
{
	public abstract class ItemSearchResult<TDocument, T> : IItemResult<T> where TDocument : class
	{
		#region [ Constructors ]s

	    protected ItemSearchResult(ISearchResult<TDocument> nativeSearchResult)
		{
		    NativeSearchResult = nativeSearchResult;
		} 

		#endregion

		#region [ Properties ]

	    public int TotalHits
	    {
	        get { return NativeSearchResult.TotalHits; }
	    }

	    public long ElapsedTimeMs { get; set; }


		public List<T> Items { get; private set; }

        protected ISearchResult<TDocument> NativeSearchResult { get; private set; } 

		#endregion

        #region [ Enumerable Methods ]

	    public IEnumerator<T> GetEnumerator()
        {
	        if (Items == null)
	        {
	            Items = GetTopItems().Items;
	        }
            return Items.GetEnumerator();
        }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return GetEnumerator();
        }

        #endregion

        #region [ Methods ]

        public virtual IItemResult<T> GetTopItems()
	    {
	        var topItems = NativeSearchResult.GetTopItems();
            ElapsedTimeMs = NativeSearchResult.ElapsedTimeMs;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
	        var results = GetResults(topItems);
            return new ItemResult<T>(results, NativeSearchResult.TotalHits) { ElapsedTimeMs = stopwatch.ElapsedMilliseconds };

	    }

        private List<T> GetResults(IEnumerable<TDocument> items)
	    {
	        return items == null ? null : items.Select(GetItem).ToList();
	    }

        /// <summary>
        /// Gets a range of items.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
	    public virtual IItemResult<T> GetRange(int start, int end)
	    {
            var pagedItems = NativeSearchResult.GetRange(start, end);
            ElapsedTimeMs = NativeSearchResult.ElapsedTimeMs;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var results = GetResults(pagedItems);
            return new ItemResult<T>(results, NativeSearchResult.TotalHits) { ElapsedTimeMs = stopwatch.ElapsedMilliseconds };
	    }

	    public abstract T GetItem(TDocument document);

        #endregion
    }
}
