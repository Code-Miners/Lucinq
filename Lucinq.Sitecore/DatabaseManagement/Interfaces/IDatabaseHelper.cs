using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Lucinq.SitecoreIntegration.DatabaseManagement.Interfaces
{
	public interface IDatabaseHelper
	{
		Language DefaultLanguage { get; }

		Database GetDatabase(string name = null);

		Item GetItem(ID itemId, Language language, string databaseName = null);

		Item GetItem(ID itemId, string databaseName = null);

		Item GetItem(string itemPath, Language language, string databaseName = null);
	}
}
