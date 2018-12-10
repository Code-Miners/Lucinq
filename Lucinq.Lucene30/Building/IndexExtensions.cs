using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Lucinq.Lucene30.Building
{
	public static class IndexExtensions
	{
		public static IndexWriter AddDocument(this IndexWriter indexWriter, params Action<Document>[] documentActions )
		{
			Document document = new Document();
			document.Setup(documentActions);
			indexWriter.AddDocument(document);
			return indexWriter;
		}
	}
}
