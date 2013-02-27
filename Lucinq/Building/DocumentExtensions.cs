using System;
using Lucene.Net.Documents;

namespace Lucinq.Building
{
	public static class DocumentExtensions
	{
		public static Document AddAnalysedField(this Document document, string fieldName, string value, bool store = false, bool caseSensitive = false)
		{
			Field.Store luceneStore = GetStoreValue(store);
			return AddField(document, fieldName, value, caseSensitive, luceneStore, Field.Index.ANALYZED);
		}

		public static Document AddNonAnalysedField(this Document document, string fieldName, string value, bool store = false, bool caseSensitive = false)
		{
			Field.Store luceneStore = GetStoreValue(store);
			return AddField(document, fieldName, value, caseSensitive, luceneStore, Field.Index.NOT_ANALYZED);
		}

		/// <summary>
		/// Stores the value in the index without indexing it - RETRIEVAL FIELDS ONLY
		/// </summary>
		/// <param name="document"></param>
		/// <param name="fieldName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Document AddStoredField(this Document document, string fieldName, string value)
		{
			return AddField(document, fieldName, value, true, Field.Store.YES, Field.Index.NO);
		}

		public static Document AddField(this Document document, string fieldName, string value, bool caseSensitive, Field.Store store, Field.Index index)
		{
			if (value == null)
			{
				return document;
			}

			if (store == null)
			{
				store = Field.Store.NO;
			}

			if (!caseSensitive)
			{
				value = value.ToLower();
			}

			Field field = new Field(fieldName, value, store, Field.Index.ANALYZED);
			document.Add(field);
			return document;
		}

		public static Document Setup(this Document document, params Action<Document>[] actions)
		{
			foreach (Action<Document> item in actions)
			{
				item(document);
			}
			return document;
		}

		#region [ Helpers ]


		private static Field.Store GetStoreValue(bool store)
		{
			Field.Store luceneStore = Field.Store.NO;
			if (store)
			{
				luceneStore = Field.Store.YES;
			}
			return luceneStore;
		}

		#endregion
	}
}
