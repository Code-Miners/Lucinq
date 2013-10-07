using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lucene.Net.Documents;
using Lucinq.Interfaces;
using Lucinq.SitecoreIntegration.Constants;
using Lucinq.SitecoreIntegration.DatabaseManagement.Interfaces;
using Lucinq.SitecoreIntegration.Querying.Interfaces;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Lucinq.SitecoreIntegration.Querying
{
	public class SitecoreSearchResult : ISitecoreSearchResult
	{
		#region [ Constructors ]

		public SitecoreSearchResult(ILuceneSearchResult searchResult, IDatabaseHelper databaseHelper, SitecoreMode sitecoreMode)
		{
			DatabaseHelper = databaseHelper;
			LuceneSearchResult = searchResult;
		    SitecoreMode = sitecoreMode;
		}

		#endregion

		#region [ Properties ]

        public SitecoreMode SitecoreMode { get; private set; }

		public IDatabaseHelper DatabaseHelper { get; private set; }

		public ILuceneSearchResult LuceneSearchResult { get; private set; }

		public int TotalHits { get { return LuceneSearchResult.TotalHits; } }

		public long ElapsedTimeMs { get; set; }

		#endregion

		#region [ Methods ]

		/// <summary>
		/// Gets a list of items for the documents
		/// </summary>
		/// <returns></returns>
		public virtual ISitecoreItemResult GetPagedItems(int start, int end, int multiplier = 3)
		{
			List<Item> items = new List<Item>();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int difference = (end + 1) - start;
			int numberAdded = 0;
			int maxCycle = end + (difference*multiplier);
			// Sometimes the items aren't published to web, in this case, continue beyond the original number of results.
			// This currently cycles through x times the initial number of rows
			LuceneSearchResult.GetPagedDocuments(start, maxCycle).ForEach(
				document =>
					{
						if (numberAdded >= difference)
						{
							return;
						}
						if (!AddItem(document, items))
						{
							return;
						}
						numberAdded++;
					}
				);
			stopwatch.Stop();
			return new SitecoreItemResult(items) { ElapsedTimeMs = stopwatch.ElapsedMilliseconds, TotalHits = LuceneSearchResult.TotalHits};
		}

		/// <summary>
		/// Gets an item by its index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public virtual Item GetItem(int index = 0)
		{
			var topDocs = LuceneSearchResult.GetTopDocuments();
			if (index < 0 || index > topDocs.Count - 1)
			{
				return null;
			}
			return GetItem(topDocs[index]);
		}

		/// <summary>
		/// Gets an item from mthe document
		/// </summary>
		/// <param name="document">The lucene document to use</param>
		/// <returns></returns>
		protected virtual Item GetItem(Document document)
		{
			string itemShortId = document.GetValues(SitecoreFields.Id).FirstOrDefault();
			if (String.IsNullOrEmpty(itemShortId))
			{
				return null;
			}
			ID itemId = new ID(itemShortId);
			string language = document.GetValues(SitecoreFields.Language).FirstOrDefault();
			if (String.IsNullOrEmpty(language))
			{
				throw new Exception("The language could not be retrieved from the lucene return");
			}
			Language itemLanguage = Language.Parse(language);

			Item item = DatabaseHelper.GetItem(itemId, itemLanguage);
		    if (item == null)
		    {
		        return null;
		    }
			return item.Versions.Count > 0 ? item : null;
		}

		private bool AddItem(Document document, List<Item> items)
		{
			Item item = GetItem(document);
			if (item == null)
			{
				return false;
			}

			items.Add(item);
			return true;
		}

		#endregion
	}
}
