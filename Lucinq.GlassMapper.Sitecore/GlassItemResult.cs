using System.Collections.Generic;
using Lucinq.GlassMapper.SitecoreIntegration.Interfaces;

namespace Lucinq.GlassMapper.SitecoreIntegration
{
	public class GlassItemResult<T> : IGlassItemResult<T>
	{
		#region [ Constructors ]

		public GlassItemResult(List<T> items, int totalHits)
		{
			Items = items;
			TotalHits = totalHits;
		} 

		#endregion

		#region [ Properties ]

		public int TotalHits { get; private set; }

		public long ElapsedTimeMs { get; set; }

		public List<T> Items { get; private set; }

		#endregion
	}
}
