namespace Lucinq.Solr.UnitTests.IntegrationTests
{
    using Core.Querying;
    using Lucene.Net.Store;
    using Lucene30.Querying;
    using NUnit.Framework;

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
