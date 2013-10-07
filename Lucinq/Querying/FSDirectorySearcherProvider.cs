using System;
using System.IO;
using Lucene.Net.Store;

namespace Lucinq.Querying
{
	 //public class FSDirectorySearcherProvider : IIndexSearcherProvider
	 //{
	 //	 private readonly FSDirectory fileSystemDirectory;

	 //	 public FSDirectorySearcherProvider(string indexPath)
	 //	 {
	 //		  fileSystemDirectory = FSDirectory.Open(indexPath);
	 //		  IndexSearcher = new IndexSearcher(fileSystemDirectory);
	 //	 }

	 //	 public void Dispose()
	 //	 {
	 //		  IndexSearcher.Dispose();
	 //		  fileSystemDirectory.Dispose();
	 //	 }

	 //	 public IndexSearcher IndexSearcher { get; private set; }
	 //}

	public class FSDirectorySearcherProvider : DirectorySearchProvider
	{
		public FSDirectorySearcherProvider(String indexPath) : base(FSDirectory.Open(new DirectoryInfo(indexPath)))
		{
			
		}
	}
}
