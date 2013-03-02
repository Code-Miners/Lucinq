using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
	public class LegacyLuceneSearchResult : ILuceneSearchResult<Hits>
	{
		#region [ Constructors ]

		public LegacyLuceneSearchResult(ILuceneSearcherAccessor luceneSearcherAccessor, Hits hits)
		{
			Results = hits;
			LuceneSearcherAccessor = luceneSearcherAccessor;
		}

		#endregion

		#region [ Properties ]

		public int TotalHits
		{
			get { return Results.Length(); }
		}

		public Hits Results { get; private set; }

		public long ElapsedTimeMs { get; set; }

		#endregion

		#region [ Methods ] 

		protected ILuceneSearcherAccessor LuceneSearcherAccessor { get; private set; }

		public List<Document> GetTopDocuments()
		{
			throw new NotSupportedException("Top documents is not supported by earlier versions of lucene");
		}

		public List<Document> GetPagedDocuments(int start, int end)
		{
			List<Document> documents = new List<Document>();
			if (start < 0)
			{
				start = 0;
			}

			if (end > Results.Length() - 1)
			{
				end = Results.Length() - 1;
			}

			for (var i = start; i <= end; i++)
			{
				documents.Add(GetDocument(i));
			}

			return documents;
		}

		public Document GetDocument(int documentId)
		{
			return Results.Doc(documentId);
		}

		#endregion
	}
}
