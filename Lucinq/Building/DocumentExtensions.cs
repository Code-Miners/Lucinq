using System;
using Lucene.Net.Documents;

namespace Lucinq.Building
{
	public static class DocumentExtensions
	{
		/// <summary>
		/// Adds a field to the index analysing its content according to the index writer's analyzer
		/// Standard Analyzer will remove much of the punctation
		/// </summary>
		/// <param name="document">The document to add the field to </param>
		/// <param name="fieldName">The name of the field to add</param>
		/// <param name="value">The value of the field</param>
		/// <param name="store">A boolean denoting whether to store the value in the index - allows retrieval of the original value from the index</param>
		/// <param name="caseSensitive">Whether to store the value in its original case</param>
		/// <returns>The input document object</returns>
		public static Document AddAnalysedField(this Document document, string fieldName, string value, bool store = false, bool caseSensitive = false)
		{
			Field.Store luceneStore = GetStoreValue(store);
			return AddField(document, fieldName, value, caseSensitive, luceneStore, Field.Index.ANALYZED);
		}

		/// <summary>
		/// Adds a non analysed field to the index - the field acts as a complete value in the index, therefore will not be stripped of punctuation / whitespace
		/// and can be searched in its original entirity
		/// </summary>
		/// <param name="document">The document to add the field to </param>
		/// <param name="fieldName">The name of the field to add</param>
		/// <param name="value">The value of the field</param>
		/// <param name="store">A boolean denoting whether to store the value in the index - allows retrieval of the original value from the index</param>
		/// <param name="caseSensitive">Whether to store the value in its original case</param>
		/// <returns>The input document object</returns>
		public static Document AddNonAnalysedField(this Document document, string fieldName, string value, bool store = false, bool caseSensitive = false)
		{
			Field.Store luceneStore = GetStoreValue(store);
			return AddField(document, fieldName, value, caseSensitive, luceneStore, Field.Index.NOT_ANALYZED);
		}

		/// <summary>
		/// Stores the value in the index without indexing it - RETRIEVAL FIELDS ONLY
		/// </summary>
		/// <param name="document">The document to add the field to </param>
		/// <param name="fieldName">The name of the field to add</param>
		/// <param name="value">The value of the field</param>
		/// <returns>The input document object</returns>
		public static Document AddStoredField(this Document document, string fieldName, string value)
		{
			return AddField(document, fieldName, value, true, Field.Store.YES, Field.Index.NO);
		}

		/// <summary>
		/// Adds a field to the index, bit more configurable than the other helper methods, but more verbose as a consequence.
		/// </summary>
		/// <param name="document">The document to add the field to </param>
		/// <param name="fieldName">The name of the field to add</param>
		/// <param name="value">The value of the field</param>
		/// <param name="store">A boolean denoting whether to store the value in the index - allows retrieval of the original value from the index</param>
		/// <param name="caseSensitive">Whether to store the value in its original case</param>
		/// <param name="index">The type of indexing to apply to the field</param>
		/// <returns>The input document object</returns>
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

		/// <summary>
		/// Sets up an already existing document with the specified actions
		/// </summary>
		/// <param name="document"></param>
		/// <param name="documentActions"></param>
		/// <returns></returns>
		public static Document Setup(this Document document, params Action<Document>[] documentActions)
		{
			foreach (Action<Document> item in documentActions)
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
