using System;
using System.Diagnostics;
using Lucene.Net.Search;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
	public class LuceneSearch : ILuceneSearch<LuceneSearchResult>, IIndexSearcherAccessor
    {
        #region [ Fields ]  

	    private readonly string indexPath;

        #endregion

        #region [ Constructors ]

        public LuceneSearch(string indexPath)
        {
            this.indexPath = indexPath;
        }

		#endregion

		#region [ Methods ]

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="customCollector"></param>
		/// <param name="filter"></param>
		public virtual void Collect(Query query, Collector customCollector, Filter filter = null)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

		    using (var indexSearcherProvider = GetIndexSearcherProvider())
		    {
		        if (filter == null)
		        {
                    indexSearcherProvider.IndexSearcher.Search(query, customCollector);
		        }
		        else
		        {
                    indexSearcherProvider.IndexSearcher.Search(query, filter, customCollector);
		        }

		        stopwatch.Stop();
		    }
		}

		public virtual LuceneSearchResult Execute(Query query, int noOfResults = Int32.MaxValue - 1, Sort sort = null, Filter filter = null)
		{
		    return new LuceneSearchResult(this, query, sort);
		}

		public LuceneSearchResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1)
		{
			return Execute(queryBuilder.Build(), noOfResults, queryBuilder.CurrentSort, queryBuilder.CurrentFilter);
		}

		public virtual void BuildSort()
		{
			
		}

        public virtual IIndexSearcherProvider GetIndexSearcherProvider()
        {
            return new FSDirectorySearcherProvider(indexPath);
        }

        /*public virtual IQueryable<Document> GetQueryable()
	    {
	        return new LuceneQueryable<Document>(this);
	    }*/

		#endregion
	}
}
