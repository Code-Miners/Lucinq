using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
	public abstract class ItemResult<T> : ILuceneSearchResult<T>
	{
		#region [ Constructors ]

	    protected ItemResult(ILuceneSearchResult<Document> luceneSearchResult)
		{
		    LuceneSearchResult = luceneSearchResult;
		} 

		#endregion

		#region [ Properties ]

	    public int TotalHits
	    {
	        get { return LuceneSearchResult.TotalHits; }
	    }

	    public long ElapsedTimeMs { get; set; }

		public List<T> Items { get; private set; }

        protected ILuceneSearchResult<Document> LuceneSearchResult { get; private set; } 

		#endregion

        #region [ Enumerable Methods ]

	    public IEnumerator<T> GetEnumerator()
        {
	        if (Items == null)
	        {
	            Items = GetTopItems();
	        }
            return Items.GetEnumerator();
        }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return GetEnumerator();
        }

        #endregion

        #region [ Methods ]

        public virtual List<T> GetTopItems()
	    {
	        var topItems = LuceneSearchResult.GetTopItems();
	        return topItems == null ? null : GetResults(topItems);
	    }

	    private List<T> GetResults(IEnumerable<Document> topItems)
	    {
	        return topItems.Select(GetItem).ToList();
	    }

	    public virtual List<T> GetPagedItems(int start, int end)
	    {
	        var pagedItems = LuceneSearchResult.GetPagedItems(start, end);
	        return pagedItems == null ? null : GetResults(pagedItems);
	    }

	    public abstract T GetItem(Document document);

        #endregion
    }
}
