using System;
using Lucinq.SitecoreIntegration.DatabaseManagement.Interfaces;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Lucinq.SitecoreIntegration.DatabaseManagement
{
	public class DatabaseHelper : IDatabaseHelper
	{
		#region [ Fields ]

		private Language defaultLanguage;

		#endregion

        #region [ Constructor ]

	    public DatabaseHelper() : this("web")
	    {
	        
	    }

	    public DatabaseHelper(string defaultDatabase)
	    {
	        DefaultDatabase = defaultDatabase;
	    }

        #endregion

        #region [ Properties ]

        public string DefaultDatabase { get; private set; }

	    public virtual Language DefaultLanguage
	    {
	        get { return defaultLanguage ?? (defaultLanguage = Sitecore.Context.Language ?? Language.Parse("en")); }
	    }
		
		#endregion

		#region [ Methods ]

		public virtual Database GetDatabase(string name = null)
		{
		    if (!String.IsNullOrEmpty(name))
		    {
		        return Database.GetDatabase(name);
		    }
            return Sitecore.Context.Database ?? Database.GetDatabase(DefaultDatabase);
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
