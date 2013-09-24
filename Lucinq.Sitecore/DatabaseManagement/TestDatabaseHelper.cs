using System;
using Lucinq.SitecoreIntegration.DatabaseManagement.Interfaces;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Lucinq.SitecoreIntegration.DatabaseManagement
{
	public class TestDatabaseHelper : IDatabaseHelper
	{
		#region [ Fields ]

		private Language defaultLanguage;

		#endregion
		
		#region [ Properties ]

		public virtual Language DefaultLanguage { get { return defaultLanguage ?? (defaultLanguage = Language.Parse("en")); } }

		#endregion

		#region [ Methods ]

		public virtual Database GetDatabase(string name = null)
		{
			if (String.IsNullOrEmpty(name))
			{
				name = "web";
			}
			return Database.GetDatabase(name);
		}

		public virtual Item GetItem(ID itemId, Language language, string databaseName = null)
		{
			if (language == null)
			{
				Console.WriteLine("Language was not specified for {0}", itemId);
				language = DefaultLanguage;
			}
			return GetDatabase(databaseName).GetItem(itemId, language);
		}

		public virtual Item GetItem(ID itemId, string databaseName = null)
		{
			return GetItem(itemId, DefaultLanguage, databaseName);
		}

		public virtual Item GetItem(string itemPath, Language language, string databaseName = null)
		{
			return GetDatabase(databaseName).GetItem(itemPath, language);
		}

		#endregion
	}
}
