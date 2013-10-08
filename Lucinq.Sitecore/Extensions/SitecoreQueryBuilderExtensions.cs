using System;
using Sitecore.Data;

namespace Lucinq.SitecoreIntegration.Extensions
{
	public static class SitecoreQueryBuilderExtensions
	{
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
