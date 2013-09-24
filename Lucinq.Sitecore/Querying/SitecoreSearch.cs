using System;
using Lucene.Net.Search;
using Lucinq.Interfaces;
using Lucinq.Querying;
using Lucinq.SitecoreIntegration.DatabaseManagement;
using Lucinq.SitecoreIntegration.DatabaseManagement.Interfaces;
using Lucinq.SitecoreIntegration.Querying.Interfaces;

namespace Lucinq.SitecoreIntegration.Querying
{
	public class SitecoreSearch : ISitecoreSearch
	{
		#region [ Constructors ]

		/// <summary>
		/// Convenience constructor with a default context database helper
		/// </summary>
		/// <param name="indexPath">The path to the index</param>
		public SitecoreSearch(string indexPath) : this(indexPath, new ContextDatabaseHelper())
		{
			
		}

		/// <summary>
		/// Allows for dependency injected alternative database helpers
		/// </summary>
		/// <param name="indexPath"></param>
		/// <param name="databaseHelper"></param>
		public SitecoreSearch(string indexPath, IDatabaseHelper databaseHelper) : this(new LuceneSearch(indexPath), databaseHelper)
		{
			
		}
		
		public SitecoreSearch(ILuceneSearch<LuceneSearchResult> luceneSearch, IDatabaseHelper databaseHelper)
		{
			LuceneSearch = luceneSearch;
			DatabaseHelper = databaseHelper;
		}

		#endregion

		#region [ Properties ]

		/// <summary>
		/// Gets the lucinq lucene search object
		/// </summary>
		public ILuceneSearch<LuceneSearchResult> LuceneSearch { get; private set; }

		/// <summary>
		/// Gets the database helper object
		/// </summary>
		public IDatabaseHelper DatabaseHelper { get; private set; }

		#endregion

		#region [ Methods ]

		public ISitecoreSearchResult Execute(Query query, int noOfResults = Int32.MaxValue - 1, Sort sort = null)
		{
			var luceneResult = LuceneSearch.Execute(query, noOfResults, sort);
			return new SitecoreSearchResult(luceneResult, DatabaseHelper) { ElapsedTimeMs = luceneResult.ElapsedTimeMs };
		}

		public ISitecoreSearchResult Execute(IQueryBuilder queryBuilder, int noOfResults = Int32.MaxValue - 1, Sort sort = null)
		{
			return Execute(queryBuilder.Build(), noOfResults, sort);
		}

		public void Dispose()
		{
			LuceneSearch.IndexSearcher.Dispose();
		}

		#endregion
	}
}
