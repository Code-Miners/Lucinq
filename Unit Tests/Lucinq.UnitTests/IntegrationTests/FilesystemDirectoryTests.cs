using System.IO;
using Lucene.Net.Store;
using Lucinq.Querying;
using NUnit.Framework;

namespace Lucinq.UnitTests.IntegrationTests
{
    [TestFixture]
    public class FilesystemDirectoryTests
    {
        [Test]
        public void OpenAndSearchOnFileSystemUsingPath()
        {
            LuceneSearch search = new LuceneSearch(GeneralConstants.Paths.CarDataIndex);
            QueryBuilder builder = new QueryBuilder(x => x.Term(CarDataFields.Make, "ford"));
            var result = search.Execute(builder);
            Assert.Greater(result.TotalHits, 0);
        }

        [Test]
        public void OpenAndSearchOnFileSystemUsingFSDirectory()
        {
            using (var directory = FSDirectory.Open(GeneralConstants.Paths.CarDataIndex))
            {
                LuceneSearch search = new LuceneSearch(directory);
                QueryBuilder builder = new QueryBuilder(x => x.Term(CarDataFields.Make, "ford"));
                var result = search.Execute(builder);
                Assert.Greater(result.TotalHits, 0);
            }
        }
    }
}
