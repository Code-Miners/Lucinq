using Sitecore.Data;

namespace Lucinq.SitecoreIntegration.Indexing
{
	public class FieldConfiguration
	{
		public ID FieldId { get; set; }

		public bool Store { get; set; }

		public bool Analyze { get; set; }
	}
}
