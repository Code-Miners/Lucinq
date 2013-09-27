using System;
using Lucinq.Interfaces;
using Lucinq.Querying;

namespace Lucinq.UnitTests.IntegrationTests
{
    using NUnit.Framework;

    [TestFixture]
	public class BasicCollectorTests
	{
		private static readonly LuceneSearch MemorySearch = new LuceneSearch(GeneralConstants.Paths.BBCIndex, true);
		private static LuceneSearch filesystemSearch = new LuceneSearch(GeneralConstants.Paths.BBCIndex);
		static readonly LuceneSearch[] Searches = new[] { filesystemSearch, MemorySearch };

		[TestFixtureSetUp]
		public void Setup()
		{
			filesystemSearch = new LuceneSearch(GeneralConstants.Paths.BBCIndex);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			filesystemSearch.Dispose();
		}

		[Test, TestCaseSource("Searches")]
		public void CollectDailyCount(LuceneSearch luceneSearch)
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.PublishDateString, "*")
				);

			DateCollector collector = new DateCollector();
			luceneSearch.Collect(queryBuilder.Build(), collector);

			foreach (String day in collector.DailyCount.Keys)
			{
				Console.Error.WriteLine("Day: {0} had {1} documents", day, collector.DailyCount[day]);
			}

			Console.WriteLine();
		}
	}
}
