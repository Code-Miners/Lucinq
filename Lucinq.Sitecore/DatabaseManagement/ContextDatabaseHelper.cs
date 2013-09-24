using System;
using Lucinq.SitecoreIntegration.DatabaseManagement.Interfaces;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Lucinq.SitecoreIntegration.DatabaseManagement
{
	public class ContextDatabaseHelper : IDatabaseHelper
	{
		#region [ Fields ]

		private Language defaultLanguage;

		#endregion

		#region [ Properties ]

		public virtual Language DefaultLanguage { get { return defaultLanguage ?? (defaultLanguage = Sitecore.Context.Language ); } }
		
		#endregion

		#region [ Methods ]

		public virtual Database GetDatabase(string name = null)
		{
			return !String.IsNullOrEmpty(name) 
				? Database.GetDatabase(name) 
				: Sitecore.Context.Database;
		}

		public virtual Item GetItem(ID itemId, Language language, string databaseName = null)
		{
			return GetDatabase(databaseName).GetItem(itemId, language);
		}

		public virtual Item GetItem(ID itemId, string databaseName = null)
		{
			Language language = DefaultLanguage;
			return GetItem(itemId, language, databaseName);
		}

		public virtual Item GetItem(string itemPath, Language language, string databaseName = null)
		{
			return GetDatabase(databaseName).GetItem(itemPath, language);
		}

		#endregion
	}
}
