using System;
using Lucene.Net.Search;

namespace Lucinq.Querying
{
	public static class DateRangeFilter
	{
		public static NumericRangeFilter<long> Filter(String field, DateTime start, DateTime end)
		{
			return NumericRangeFilter.NewLongRange(field, start.Ticks, end.Ticks, true, true);
		}
	}
}
