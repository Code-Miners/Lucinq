using System;
using System.Diagnostics;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
	public class LuceneSearch : ILuceneSearch<LuceneSearchResult>, IIndexSearcherAccessor
    {
        #region [ Fields ]  

	    private readonly string indexPath;

		 private Directory IndexDirectory { get; set; }

        #endregion

        #region [ Constructors ]

        public LuceneSearch(String indexPath)
        {
            this.indexPath = indexPath;
        }

		public LuceneSearch(Directory indexDirectory)
		{
			IndexDirectory = indexDirectory;
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
		    return new LuceneSearchResult(this, query, sort, filter);
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
			  if (IndexDirectory == null)
			  {
				  return new FSDirectorySearcherProvider(indexPath);
			  }

			  return new DirectorySearchProvider(IndexDirectory);
        }


		#endregion
	}
}
