using System;
using System.Diagnostics;
using System.IO;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
	public class LuceneSearch : ILuceneSearch<LuceneSearchResult>
	{
		#region [ Constructors ]

		public LuceneSearch(string indexPath)
			: this(indexPath, false)
		{

		}

		public LuceneSearch(string indexPath, bool useRamDirectory)
		{
			UseRamDirectory = useRamDirectory;
			FileSystemDirectory = FSDirectory.Open(new DirectoryInfo(indexPath));
			if (useRamDirectory)
			{
				RamDirectory = new RAMDirectory(FileSystemDirectory);
				IndexSearcher = new IndexSearcher(RamDirectory, true);
			}
			else
			{
				IndexSearcher = new IndexSearcher(FileSystemDirectory, true);
			}
		}
		#endregion

		#region [ Properties ]

		public IndexSearcher IndexSearcher { get; private set; }

		protected FSDirectory FileSystemDirectory { get; private set; }

		protected RAMDirectory RamDirectory { get; private set; }

		protected bool UseRamDirectory { get; private set; }

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

			if (filter == null)
			{
				IndexSearcher.Search(query, customCollector);
			}
			else
			{
				IndexSearcher.Search(query, filter, customCollector);
			}

			stopwatch.Stop();
		}

		public virtual LuceneSearchResult Execute(Query query, int noOfResults = Int32.MaxValue - 1, Sort sort = null, Filter filter = null)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			if (sort == null)
			{
				sort = Sort.RELEVANCE;
			}
			
			TopDocs topDocs = IndexSearcher.Search(query, filter, noOfResults, sort);
			stopwatch.Stop();
			LuceneSearchResult searchResult = new LuceneSearchResult(this, topDocs)
				{
					ElapsedTimeMs = stopwatch.ElapsedMilliseconds
				};
			return searchResult;
		}

		public LuceneSearchResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1)
		{
			return Execute(queryBuilder.Build(), noOfResults, queryBuilder.CurrentSort, queryBuilder.CurrentFilter);
		}

		public virtual void BuildSort()
		{
			
		}

        /*public virtual IQueryable<Document> GetQueryable()
	    {
	        return new LuceneQueryable<Document>(this);
	    }*/

		#endregion

		public void Dispose()
		{
			try
			{
				if (FileSystemDirectory != null)
				{
					FileSystemDirectory.Dispose();
				}
			}
			finally
			{

			}

			try
			{
				if (RamDirectory != null)
				{
					RamDirectory.Dispose();
				}
			}
			finally
			{

			}

			try
			{
				if (IndexSearcher != null)
				{
                    IndexSearcher.Dispose();
				}
			}
			finally
			{

			}
		}
	}
}
