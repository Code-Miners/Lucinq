using NUnit.Framework;
using Sitecore.Search;

namespace Lucinq.Sitecore.UnitTests.IntegrationTests
{
	[TestFixture]
	public class Indexing
	{
		#region [ Constants ]

		private const string IndexName = "TestSearchIndex";

		#endregion

		[Ignore("Index doesn't need rebuilding every time")]
		[Test]
		public void RebuildSearchIndex()
		{
			Index searchIndex = SearchManager.GetIndex(IndexName);
			searchIndex.Rebuild();
		}
	}
}
