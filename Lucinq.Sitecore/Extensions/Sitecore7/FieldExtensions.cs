using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lucene.Net.Search;
using Lucinq.Enums;
using Lucinq.Interfaces;
using Sitecore.ContentSearch;

namespace Lucinq.SitecoreIntegration.Extensions.Sitecore7
{
    public static class FieldExtensions
    {
        #region [ Terms ]

        public static TermQuery Term<T>(this IQueryBuilderIndividual queryBuilder, Expression<Func<T, object>> expression, string fieldValue, Matches occur = Matches.NotSet, float? boost = null,
            string key = null, bool? caseSensitive = null)
        {
            return queryBuilder.Term(GetFieldName(expression), fieldValue, occur, boost, key, caseSensitive);
        }

        #endregion

        #region [ WildCard ]

        public static WildcardQuery WildCard<T>(this IQueryBuilderIndividual queryBuilder, Expression<Func<T, object>> expression, string fieldValue, Matches occur = Matches.NotSet, float? boost = null,
            string key = null, bool? caseSensitive = null)
        {
            return queryBuilder.WildCard(GetFieldName(expression), fieldValue, occur, boost, key, caseSensitive);
        }

        #endregion

        #region [ Field ]

        public static Query Field<T>(this IQueryBuilderIndividual queryBuilder, Expression<Func<T, object>> expression, string fieldValue, Matches occur = Matches.NotSet, float? boost = null,
            string key = null, bool? caseSensitive = null, int slop = 1)
        {
            return queryBuilder.Field(GetFieldName(expression), fieldValue, occur, boost, key, slop);
        }

        #endregion

        #region [ Keyword ]

        public static Query Keyword<T>(this IQueryBuilderIndividual queryBuilder, Expression<Func<T, object>> expression, string fieldValue, Matches occur = Matches.NotSet, float? boost = null,
            string key = null, bool? caseSensitive = null)
        {
            return queryBuilder.Keyword(GetFieldName(expression), fieldValue, occur, boost, key, caseSensitive);
        }

        #endregion

        #region [ Field Name Extensions ]

        public static string GetFieldName<T>(this T target, Expression<Func<T, object>> field)
        {
            return GetFieldName<T>(field);
        }

        public static string GetFieldName<T>(Expression<Func<T, object>> field)
        {
            if (field.Parameters.Count > 1)
            {
                throw new ApplicationException(String.Format("To many parameters in linq expression {0}", field.Body));
            }

            MemberExpression memberExpression;

            UnaryExpression body = field.Body as UnaryExpression;
            if (body != null)
            {
                memberExpression = body.Operand as MemberExpression;
            }
            else if (!(field.Body is MemberExpression))
            {
                throw new ApplicationException(String.Format("Expression doesn't evaluate to a member {0}", field.Body));
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

            //lambda expression does not always return expected memberinfo when inheriting
            //c.f. http://stackoverflow.com/questions/6658669/lambda-expression-not-returning-expected-memberinfo
            var prop = type.GetProperty(memberExpression.Member.Name);

            //interfaces don't deal with inherited properties well
            if (prop == null && type.IsInterface)
            {
                Func<Type, PropertyInfo> interfaceCheck = null;
                interfaceCheck = inter =>
                {
                    var interfaces = inter.GetInterfaces();
                    var properties =
                        interfaces.Select(x => x.GetProperty(memberExpression.Member.Name)).Where(
                            x => x != null);
                    PropertyInfo property = properties.FirstOrDefault();
                    return property ?? interfaces.Select(x => interfaceCheck(x)).FirstOrDefault(x => x != null);
                };
                prop = interfaceCheck(type);
            }

            if (prop != null && prop.DeclaringType != null && prop.DeclaringType != prop.ReflectedType)
            {
                //properties mapped in data handlers are based on declaring type when field is inherited, make sure we match
                prop = prop.DeclaringType.GetProperty(prop.Name);
            }

            if (prop == null)
                throw new ApplicationException(String.Format("Error. Could not find property {0} on type {1}", memberExpression.Member.Name, type.FullName));

            var indexFieldAttribute = prop.GetCustomAttribute<IndexFieldAttribute>(true);

            return indexFieldAttribute != null ? indexFieldAttribute.IndexFieldName : prop.Name.ToLowerInvariant();
        }

        #endregion

    }
}
