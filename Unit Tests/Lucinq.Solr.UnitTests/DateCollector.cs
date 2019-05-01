namespace Lucinq.Solr.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Lucene.Net.Index;
    using Lucene.Net.Search;

    public class DateCollector : Collector
	{
		public int Count { get; private set; }

		private long[] dates;

		public Dictionary<String, int> DailyCount { get; set; }

		public DateCollector()
		{
			//Years = new Dictionary<String, Dictionary<String, int>>();
			dates = new long[10];
			DailyCount = new Dictionary<String, int>();
		}

		public void Reset()
		{
			Count = 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="docId"></param>
		public override void Collect(int docId)
		{
			Count = Count + 1;

			try
			{
				long temp = dates[docId];

				DateTime date = new DateTime(temp);
				String day = date.DayOfWeek.ToString();

				if (!DailyCount.ContainsKey(day))
				{
					DailyCount[day] = 1;
				}
				else
				{
					DailyCount[day]++;
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.ToString());
			}
			
		}

		public override void SetScorer(Scorer scorer) { }

		public override void SetNextReader(IndexReader reader, int docBase)
		{
            dates = FieldCache_Fields.DEFAULT.GetLongs(reader, BBCFields.PublishDateObject);
		}

	    public override bool AcceptsDocsOutOfOrder
	    {
            get { return true; }
	    }
	}
}
