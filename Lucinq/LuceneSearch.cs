using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Interfaces;

namespace Lucinq
{
	public class LuceneSearch : ILuceneSearch
	{
		#region [ Constructors ]

		public LuceneSearch(string indexPath)
		{
			IndexSearcher = new IndexSearcher(FSDirectory.Open(new DirectoryInfo(indexPath)), true);
		}

		#endregion

		#region [ Properties ]

		public IndexSearcher IndexSearcher { get; private set; }

		#endregion

		#region [ Methods ]

		public TopDocs Execute(Query query, int noOfResults)
		{
			return IndexSearcher.Search(query, null, noOfResults);
		}

		public Document GetDocument(int documentId)
		{
			return IndexSearcher.Doc(documentId);
		}

		public List<Document> GetTopDocuments(TopDocs topDocs)
		{
			return topDocs == null ? null : (from ScoreDoc doc in topDocs.ScoreDocs select GetDocument(doc.doc)).ToList();
		}

		#endregion
	}
}
