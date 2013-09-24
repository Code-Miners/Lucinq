using System;
using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Lucinq.Enums;
using Lucinq.Extensions;
using Lucinq.Interfaces;
using Lucinq.SitecoreIntegration.Constants;
using Sitecore.Data;
using Sitecore.Globalization;

namespace Lucinq.SitecoreIntegration.Extensions
{
	public static class SitecoreQueryBuilderExtensions
	{
		#region [ ID Extensions ]

        public static TermQuery Id(this IQueryBuilder inputQueryBuilder, ID itemId, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			string luceneItemId = itemId.ToLuceneId();
			return inputQueryBuilder.Term(SitecoreFields.Id, luceneItemId, occur, boost, key);
		}

        public static IQueryBuilder Ids(this IQueryBuilder inputQueryBuilder, ID[] itemIds, float? boost = null, Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, string key = null)
		{
			var group = inputQueryBuilder.Group(occur);
			foreach (ID itemId in itemIds)
			{
				group.Id(itemId, childrenOccur, boost, key);
			}
			return inputQueryBuilder;
		}

		#endregion 

		#region [ Template Extensions ]

		/// <summary>
		/// To find items that directly inherit from a particular template
		/// </summary>
		/// <param name="inputQueryBuilder"></param>
		/// <param name="templateId"></param>
		/// <param name="occur"></param>
		/// <param name="boost"></param>
		/// <param name="key"></param>
		/// <returns></returns>
        public static TermQuery TemplateId(this IQueryBuilder inputQueryBuilder, ID templateId, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			string luceneTemplateId = templateId.ToLuceneId();
			return inputQueryBuilder.Term(SitecoreFields.TemplateId, luceneTemplateId, occur, boost, key);
		}

		/// <summary>
		/// Helper to add multiple templates
		/// </summary>
		/// <param name="inputQueryBuilder"></param>
		/// <param name="templateIds"></param>
		/// <param name="boost"></param>
		/// <param name="occur"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static IQueryBuilder TemplateIds(this IQueryBuilder inputQueryBuilder, ID[] templateIds, float? boost = null, Matches occur = Matches.NotSet, Matches childrenOccur = Matches.NotSet, string key = null)
		{
			var group = inputQueryBuilder.Group(occur);
			foreach (ID templateId in templateIds)
			{
				group.TemplateId(templateId, childrenOccur, boost, key);
			}
			return inputQueryBuilder;
		}

		/// <summary>
		/// Get items derived from the given template at some point in their heirarchy
		/// </summary>
		/// <param name="inputQueryBuilder"></param>
		/// <param name="templateId"></param>
		/// <param name="occur"></param>
		/// <param name="boost"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static TermQuery BaseTemplateId(this IQueryBuilder inputQueryBuilder, ID templateId, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			string luceneTemplateId = templateId.ToLuceneId();
			return inputQueryBuilder.Term(SitecoreFields.TemplatePath, luceneTemplateId, occur, boost, key);
		}
		
		#endregion

		#region [ Name Extensions ]

		public static Query Name(this IQueryBuilder inputQueryBuilder, string value, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			return inputQueryBuilder.Term(SitecoreFields.Name, value, occur, boost, key);
		}

		public static IQueryBuilder Names(this IQueryBuilder inputQueryBuilder, string[] values, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			foreach (string templateId in values)
			{
				inputQueryBuilder.Term(SitecoreFields.TemplateId, templateId, occur, boost, key);
			}
			return inputQueryBuilder;
		}

		#endregion

		#region [ Regionalisation ]

		public static Query Language(this IQueryBuilder inputQueryBuilder, Language language, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			string languageString = language.CultureInfo.Name.ToLower();
			return inputQueryBuilder.Keyword(SitecoreFields.Language, languageString, occur, boost, key);
		}

		public static IQueryBuilder Languages(this IQueryBuilder inputQueryBuilder, Language[] languages, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			foreach (Language language in languages)
			{
				inputQueryBuilder.Language(language, occur, boost, key);
			}
			return inputQueryBuilder;
		}
		#endregion

		#region [ Heirarchy Extensions ]

		/// <summary>
		/// Gets a query representing 
		/// </summary>
		/// <param name="inputQueryBuilder"></param>
		/// <param name="ancestorId"></param>
		/// <param name="occur"></param>
		/// <param name="boost"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static TermQuery Ancestor(this IQueryBuilder inputQueryBuilder, ID ancestorId, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			string ancestorIdString = ancestorId.ToLuceneId();
			return inputQueryBuilder.Term(SitecoreFields.Path, ancestorIdString, occur, boost, key);
		}

        public static TermQuery Parent(this IQueryBuilder inputQueryBuilder, ID parentId, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			string parentIdString = parentId.ToLuceneId();
			return inputQueryBuilder.Term(SitecoreFields.Parent, parentIdString, occur, boost, key);
		}

		#endregion

		#region [ Field Extensions ]

        public static Query Field(this IQueryBuilderIndividual inputQueryBuilder, string fieldName, string value, Matches occur = Matches.NotSet, float? boost = null, string key = null, int slop = 1)
		{
			if (value.Contains("*"))
			{
				return inputQueryBuilder.WildCard(GetEncodedFieldName(fieldName), value.ToLower(), occur, boost, key);
			}
			if (value.Contains(" "))
			{
				PhraseQuery phraseQuery = inputQueryBuilder.Phrase(slop, boost, occur, key);
				string[] valueElems = value.Split(new[] {' '});
				foreach (var valueElem in valueElems)
				{
					phraseQuery.AddTerm(GetEncodedFieldName(fieldName), valueElem.ToLower());	
				}
				return phraseQuery;
			}
			return inputQueryBuilder.Term(GetEncodedFieldName(fieldName), value.ToLower(), occur, boost, key);
		}

        public static Query Field(this IQueryBuilderIndividual inputQueryBuilder, string fieldName, ID value, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			return inputQueryBuilder.Term(GetEncodedFieldName(fieldName), value.ToLuceneId(), occur, boost, key);
		}

		#endregion

		#region [ Database ]

        public static Query Database(this IQueryBuilder inputQueryBuilder, string value, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			return inputQueryBuilder.Term(SitecoreFields.Database, value.ToLower(), occur, boost, key);
		}

		#endregion

		#region [ Id Extensions ]

		public static string ToLuceneId(this ID itemId)
		{
			return itemId.ToShortID().ToString().ToLowerInvariant();
		}

		public static string ToLuceneId(this Guid itemId)
		{
			return new ID(itemId).ToShortID().ToString().ToLowerInvariant();
		}

		public static string ToLuceneId(this string itemId)
		{
			return new ID(itemId).ToShortID().ToString().ToLowerInvariant();
		}

		public static string GetEncodedFieldName(string fieldName)
		{
			return fieldName.Replace(" ", "_").ToLower();
		}

		#endregion
	}
}
