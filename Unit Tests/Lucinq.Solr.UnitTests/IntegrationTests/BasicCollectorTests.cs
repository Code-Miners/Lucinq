namespace Lucinq.Solr.UnitTests.IntegrationTests
{
    using System;
    using Core.Interfaces;
    using Core.Querying;
    using Lucene30.Querying;
    using NUnit.Framework;

    /// <summary>
	/// 
	/// </summary>
   [TestFixture]
	public class BasicCollectorTests : BaseTestFixture
	{
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void CollectDailyCount()
		{
			LuceneSearch luceneSearch = new LuceneSearch(IndexDirectory);

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

        // todo: NM - Fix Filter
        /* 

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void CollectDailyWithFilterCount()
		{
			LuceneSearch luceneSearch = new LuceneSearch(IndexDirectory);

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
        */

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void CollectDailyCountFromQueryBuilder()
        {
            LuceneSearch luceneSearch = new LuceneSearch(IndexDirectory);

            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Setup
                (
                    x => x.WildCard(BBCFields.PublishDateString, "*")
                );

            DateCollector collector = new DateCollector();
            luceneSearch.Collect(queryBuilder, collector);

            Assert.Greater(collector.DailyCount.Keys.Count, 0);
            foreach (String day in collector.DailyCount.Keys)
            {
                Console.Error.WriteLine("Day: {0} had {1} documents", day, collector.DailyCount[day]);
            }

            Console.WriteLine();
        }
	}
}
