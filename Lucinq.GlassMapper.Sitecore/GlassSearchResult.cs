using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Glass.Mapper.Sc;
using Lucene.Net.Documents;
using Lucinq.GlassMapper.SitecoreIntegration.Interfaces;
using Lucinq.Interfaces;
using Lucinq.SitecoreIntegration.Constants;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Globalization;

namespace Lucinq.GlassMapper.SitecoreIntegration
{
	public class GlassSearchResult : IGlassSearchResult
	{
		#region [ Constructors ]

		public GlassSearchResult(ISitecoreService sitecoreService, ILuceneSearchResult luceneSearchResult)
		{
			LuceneSearchResult = luceneSearchResult;
			SitecoreService = sitecoreService;
		}

		#endregion

		#region [ Properties ]

		public int TotalHits { get { return LuceneSearchResult.TotalHits; } }

		public long ElapsedTimeMs { get; set; }

		public ISitecoreService SitecoreService { get; private set; }

		public ILuceneSearchResult LuceneSearchResult { get; private set; }

		#endregion

		#region [ Methods ]

		/// <summary>
		/// Gets a paged items
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="multiplier"></param>
		/// <returns></returns>
		public IGlassItemResult<T> GetPagedItems<T>(int start, int end, int multiplier = 3) where T : class
		{
			List<T> items = new List<T>();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int difference = (end + 1) - start;
			int numberAdded = 0;
			int maxCycle = end + (difference * multiplier);
			// Sometimes the items aren't published to web, in this case, continue beyond the original number of results.
			// This currently cycles through 3 times the initial number of rows
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
				});
			stopwatch.Stop();
			return new GlassItemResult<T>(items, LuceneSearchResult.TotalHits) { ElapsedTimeMs = stopwatch.ElapsedMilliseconds };
		}

		/// <summary>
		/// Gets all of the items related to the top documents
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IGlassItemResult<T> GetTopItems<T>() where T : class
		{
			List<T> items = new List<T>();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			LuceneSearchResult.GetTopDocuments().ForEach(document => AddItem(document, items));
			stopwatch.Stop();
			return new GlassItemResult<T>(items, LuceneSearchResult.TotalHits) { ElapsedTimeMs = stopwatch.ElapsedMilliseconds };
		}

		public T GetItem<T>(int index = 0) where T : class
		{
			var topDocs = LuceneSearchResult.GetTopDocuments();
			if (index < 0 || index > topDocs.Count - 1)
			{
				return null;
			}
			return GetItem<T>(topDocs[index]);
		}

		protected virtual T GetItem<T>(Document document) where T : class
		{
		    Type type = typeof (T);
		    if (type.IsAssignableFrom(typeof(SearchResultItem)))
		    {
		        
		    }
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

			return SitecoreService.GetItem<T>(itemId.ToGuid(), itemLanguage);
		}

		private bool AddItem<T>(Document document, ICollection<T> items) where T : class
		{
			T item = GetItem<T>(document);
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
