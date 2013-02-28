using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
	public class LuceneSearchResult : ILuceneSearchResult<TopDocs>
	{
		#region [ Constructors ]

		public LuceneSearchResult(ILuceneSearcherAccessor luceneSearcherAccessor, TopDocs topDocs)
		{
			Results = topDocs;
			LuceneSearcherAccessor = luceneSearcherAccessor;
		}

		#endregion

		#region [ Properties ]

		public int TotalHits
		{
			get { return Results.TotalHits; }
		}

		public TopDocs Results { get; private set; }

		public long ElapsedTimeMs { get; set; }

		#endregion

		#region [ Methods ] 

		protected ILuceneSearcherAccessor LuceneSearcherAccessor { get; private set; }

		public List<Document> GetTopDocuments()
		{
			return Results == null ? null : (from ScoreDoc doc in Results.ScoreDocs select GetDocument(doc.doc)).ToList();
		}

		public List<Document> GetPagedDocuments(int start, int end)
		{
			List<Document> documents = new List<Document>();
			if (start < 0)
			{
				start = 0;
			}

			if (end > Results.TotalHits - 1)
			{
				end = Results.TotalHits - 1;
			}
			if (end > Results.ScoreDocs.Length)
			{
				end = Results.ScoreDocs.Length - 1;
			}

			for (var i = start; i <= end; i++)
			{
				documents.Add(GetDocument(Results.ScoreDocs[i].doc));
			}

			return documents;
		}

		public Document GetDocument(int documentId)
		{
			return LuceneSearcherAccessor.IndexSearcher.Doc(documentId);
		}

		#endregion
	}
}
