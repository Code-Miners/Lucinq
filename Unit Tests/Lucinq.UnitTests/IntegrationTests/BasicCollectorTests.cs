using System;
using Lucinq.Interfaces;
using Lucinq.Querying;

namespace Lucinq.UnitTests.IntegrationTests
{
    using NUnit.Framework;

    [TestFixture]
	public class BasicCollectorTests
	{
		private static LuceneSearch filesystemSearch = new LuceneSearch(GeneralConstants.Paths.BBCIndex);
		static readonly LuceneSearch[] Searches = new[] { filesystemSearch };

		[TestFixtureSetUp]
		public void Setup()
		{
			filesystemSearch = new LuceneSearch(GeneralConstants.Paths.BBCIndex);
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

            Assert.Greater(collector.DailyCount.Keys.Count, 0);
			foreach (String day in collector.DailyCount.Keys)
			{
				Console.Error.WriteLine("Day: {0} had {1} documents", day, collector.DailyCount[day]);
			}

			Console.WriteLine();
		}


		[Test, TestCaseSource("Searches")]
		public void CollectDailyWithFilterCount(LuceneSearch luceneSearch)
		{
			IQueryBuilder queryBuilder = new QueryBuilder();
			queryBuilder.Setup
				(
					x => x.WildCard(BBCFields.Description, "food"),
					x => x.Filter(DateRangeFilter.Filter(BBCFields.PublishDateObject, DateTime.Parse("01/02/2013"), DateTime.Parse("28/02/2013")))
				);

			DateCollector collector = new DateCollector();
			luceneSearch.Collect(queryBuilder.Build(), collector);

			Assert.Greater(collector.DailyCount.Keys.Count, 0);
			foreach (String day in collector.DailyCount.Keys)
			{
				Console.Error.WriteLine("Day: {0} had {1} documents", day, collector.DailyCount[day]);
			}

			Console.WriteLine();
		}
	}
}
