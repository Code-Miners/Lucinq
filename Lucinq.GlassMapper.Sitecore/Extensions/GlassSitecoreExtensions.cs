using System;
using System.Linq;
using System.Linq.Expressions;
using Glass.Mapper;
using Glass.Mapper.Configuration;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration;
using Lucene.Net.Search;
using Lucinq.Enums;
using Lucinq.Interfaces;
using Lucinq.SitecoreIntegration.Constants;
using Lucinq.SitecoreIntegration.Extensions;
using Lucinq.SitecoreIntegration.Querying.Interfaces;

namespace Lucinq.GlassMapper.SitecoreIntegration.Extensions
{
    using Sitecore.Data;

    public static class GlassSitecoreExtensions
	{
		#region [ Fields ]

		private static ISitecoreService sitecoreService;

		#endregion

		#region [ Properties ]

		internal static ISitecoreService SitecoreService 
		{
			get { return sitecoreService ?? (sitecoreService = new SitecoreContext()); }
		}
		#endregion

		#region [ Field Extensions ]

        public static Query Field<T>(this ISitecoreQueryBuilder inputQueryBuilder, Expression<Func<T, object>> field, 
			string value, Matches occur = Matches.NotSet, float? boost = null, string key = null, int slop = 1)
		{
			return Field(inputQueryBuilder, field, SitecoreService, value, occur, boost, key, slop);
		}

        public static Query Field<T>(this ISitecoreQueryBuilder inputQueryBuilder, Expression<Func<T, object>> field,
                                     ISitecoreService service, string value, Matches occur = Matches.NotSet, float? boost = null, string key = null, int slop = 1)
		{
			string fieldName = GetFieldName(field, service);
			return inputQueryBuilder.Field(fieldName, value, occur, boost, key, slop);
		}

        public static Query Field<T>(this ISitecoreQueryBuilder inputQueryBuilder, Expression<Func<T, object>> field,
            ID value, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			return Field(inputQueryBuilder, field, SitecoreService, value, occur, boost, key);
		}

        public static Query Field<T>(this ISitecoreQueryBuilder inputQueryBuilder, Expression<Func<T, object>> field,
                                     ISitecoreService service, ID value, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			string fieldName = GetFieldName(field, service);
			return inputQueryBuilder.Term(SitecoreQueryBuilderExtensions.GetEncodedFieldName(fieldName), value.ToLuceneId(), occur, boost, key);
		}

		#endregion

		#region [ Keyword Extensions ]

        public static Query Keyword<T>(this IQueryBuilder inputQueryBuilder, Expression<Func<T, object>> field, string value, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			return Keyword(inputQueryBuilder, field, SitecoreService, value, occur, boost, key);
		}

		public static Query Keyword<T>(this IQueryBuilder inputQueryBuilder, Expression<Func<T, object>> field,
                             ISitecoreService service, string value, Matches occur = Matches.NotSet, float? boost = null, string key = null)
		{
			string fieldName = GetFieldName(field, service);
			return inputQueryBuilder.Keyword(SitecoreQueryBuilderExtensions.GetEncodedFieldName(fieldName), value, occur, boost, key);
		}

		#endregion

		#region [ Sort Extensions ]

		public static IQueryBuilder Sort<T>(this IQueryBuilder inputQueryBuilder, Expression<Func<T, object>> field,
											bool sortDescending = false, int? sortType = null)
		{
			return inputQueryBuilder.Sort(field, SitecoreService, sortDescending, sortType);
		}

		public static IQueryBuilder Sort<T>(this IQueryBuilder inputQueryBuilder, Expression<Func<T, object>> field, ISitecoreService service,
			bool sortDescending = false, int? sortType = null)
		{
			string fieldName = GetFieldName(field, service);
			return inputQueryBuilder.Sort(SitecoreQueryBuilderExtensions.GetEncodedFieldName(fieldName), sortDescending, sortType);
		}

		#endregion

		#region [ Field Name Extensions ]

		public static string GetFieldName<T>(Expression<Func<T, object>> field)
		{
			return GetFieldName(field, new SitecoreContext());
		}

		public static string GetFieldName<T>(Expression<Func<T, object>> field, ISitecoreService service)
		{
			if (field.Parameters.Count > 1)
				throw new MapperException("To many parameters in linq expression {0}".Formatted(field.Body));

			MemberExpression memberExpression;

			UnaryExpression body = field.Body as UnaryExpression;
			if (body != null)
			{
				memberExpression = body.Operand as MemberExpression;
			}
			else if (!(field.Body is MemberExpression))
			{
				throw new MapperException("Expression doesn't evaluate to a member {0}".Formatted(field.Body));
			}
			else
			{
				memberExpression = (MemberExpression)field.Body;
			}


			if (memberExpression == null)
			{
				return String.Empty;
			}

			//if the class a proxy then we have to get it's base type
			Type type = typeof(T);

		    AbstractTypeConfiguration typeConfig;
		    if (service.GlassContext.TypeConfigurations.TryGetValue(type, out typeConfig))
		    {
		        AbstractPropertyConfiguration propertyConfiguration =
		            typeConfig.Properties.FirstOrDefault(p => p.PropertyInfo.Name == memberExpression.Member.Name);
		        ;
		        if (propertyConfiguration is SitecoreIdConfiguration)
		        {
		            return SitecoreFields.Id;
		        }
		        if (propertyConfiguration is SitecoreInfoConfiguration)
		        {
		            SitecoreInfoConfiguration infoConfiguration = propertyConfiguration as SitecoreInfoConfiguration;
		            switch (infoConfiguration.Type)
		            {
		                case SitecoreInfoType.Language:
		                    return SitecoreFields.Language;
		                case SitecoreInfoType.Name:
		                    return SitecoreFields.Name;
		                case SitecoreInfoType.TemplateId:
		                    return SitecoreFields.TemplateId;
		                case SitecoreInfoType.Path:
		                    return SitecoreFields.Path;
		            }
		        }
		        if (propertyConfiguration is SitecoreFieldConfiguration)
		        {
		            SitecoreFieldConfiguration fieldConfiguration = propertyConfiguration as SitecoreFieldConfiguration;
		            return fieldConfiguration.FieldName;
		        }

		    }
		    else
		    {
		        throw new MapperException(
                    "Could not find data handler for property {2} {0}.{1}".Formatted(memberExpression.Member.Name, memberExpression.Member.MemberType));
		    }
			return String.Empty;
		}

		#endregion
	}
}
