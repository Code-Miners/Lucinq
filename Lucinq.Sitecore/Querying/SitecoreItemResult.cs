using System.Collections;
using System.Collections.Generic;
using Lucinq.SitecoreIntegration.Querying.Interfaces;
using Sitecore.Data.Items;

namespace Lucinq.SitecoreIntegration.Querying
{
	public class SitecoreItemResult : ISitecoreItemResult
	{
		#region [ Constructors ]

		public SitecoreItemResult(List<Item> items)
		{
			Items = items;
		}

		#endregion

		#region [ Properties ]

		public List<Item> Items { get; private set; }

		public long ElapsedTimeMs { get; set; }

		public int TotalHits { get; set; }

		#endregion

	    public IEnumerator<Item> GetEnumerator()
	    {
	        return ((IEnumerable<Item>) Items).GetEnumerator();
	    }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return GetEnumerator();
	    }
	}
}
