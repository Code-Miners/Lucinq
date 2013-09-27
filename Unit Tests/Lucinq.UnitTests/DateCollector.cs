using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace Lucinq.UnitTests
{
	public class DateCollector : Collector
	{
		public int Count { get; private set; }

		private String[] dates;

		public Dictionary<String, int> DailyCount { get; set; }

		public DateCollector()
		{
			//Years = new Dictionary<String, Dictionary<String, int>>();
			dates = new String[10];
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

			String temp = dates[docId];

			// "20130220-060258-38"

			DateTime date = DateTime.ParseExact(temp, "yyyyMMdd-HHmmss-ff", CultureInfo.InvariantCulture); // DateTime.Parse(temp, );
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

		public override void SetScorer(Scorer scorer) { }

		public override void SetNextReader(IndexReader reader, int docBase)
		{
            dates = FieldCache_Fields.DEFAULT.GetStrings(reader, BBCFields.PublishDateString);
		}

	    public override bool AcceptsDocsOutOfOrder
	    {
            get { return true; }
	    }
	}
}
