using System;
using System.IO;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
	 //public class RamDirectorySearcherProvider : IIndexSearcherProvider
	 //{
	 //	 private readonly FSDirectory fileSystemDirectory;
	 //	 private readonly RAMDirectory ramDirectory;

	 //	 public RamDirectorySearcherProvider(string indexPath)
	 //	 {
	 //		  fileSystemDirectory = FSDirectory.Open(indexPath);
	 //		  ramDirectory = new RAMDirectory(fileSystemDirectory);

	 //		  IndexSearcher = new IndexSearcher(ramDirectory);
	 //	 }

	 //	 public void Dispose()
	 //	 {
	 //		  IndexSearcher.Dispose();
	 //		  ramDirectory.Dispose();
	 //		  fileSystemDirectory.Dispose();
	 //	 }

	 //	 public IndexSearcher IndexSearcher { get; private set; }
	 //}

	public class RamDirectorySearcherProvider : DirectorySearcherProvider
	{
		public RamDirectorySearcherProvider(string indexPath) : base(new RAMDirectory(FSDirectory.Open(new DirectoryInfo(indexPath))))
		{

		}
	}
}
