using System;
using System.Linq;
using System.Text;
using Lucinq.Core.Adapters;
using Lucinq.Core.Enums;
using Lucinq.Core.Querying;
using SolrNet;

namespace Lucinq.Solr.Sitecore.Adapters
{
    public class SolrSearchAdapter : IProviderAdapter<SolrSearchModel>
    {
        protected SolrSearchModel NativeModel { get; set; }

        public virtual SolrSearchModel Adapt(LucinqQueryModel model)
        {
            NativeModel = new SolrSearchModel();
            Visit(model.Query);
            Visit(model.Sort);
            Visit(model.Filter);
            return NativeModel;
        }

        protected virtual void Visit(LucinqSort sort)
        {
            if (sort?.SortFields == null || sort.SortFields.Length == 0)
            {
                return;
            }

            if (sort?.SortFields == null || sort.SortFields.Length == 0)
            {
                return;
            }

            foreach (var sortField in sort.SortFields)
            {

                Order order = sortField.SortDescending ? Order.DESC : Order.ASC;

                NativeModel.QueryOptions.OrderBy.Add(new SortOrder(sortField.FieldName, order));

            }
        }

        protected virtual void Visit(LucinqFilter filter)
        {
            if (filter == null)
            {
                return;
            }

            string comparator;
            string filterString;
            switch (filter.Comparator)
            {
                case Comparator.Equals:
                    filterString = $"{filter.Field}:{filter.Value}";
                    break;
                case Comparator.NotEquals:
                    filterString = $"{filter.Field}:!{filter.Value}";
                    break;
                case Comparator.GreaterThan:
                    filterString = $"{filter.Field}:{{{filter.Value} TO * ]";
                    break;
                case Comparator.GreaterThanEquals:
                    filterString = $"{filter.Field}:[{filter.Value} TO * ]";
                    break;
                case Comparator.LessThan:
                    filterString = $"{filter.Field}:[* TO {filter.Value} }}";
                    break;
                case Comparator.LessThanEquals:
                    filterString = $"{filter.Field}:[* TO {filter.Value} ]";
                    break;
                default:
                    filterString = $"{filter.Field}:{filter.Value}";
                    break;
            }

            if (NativeModel.FilterBuilder.Length > 0)
            {
                NativeModel.FilterBuilder.Append(" and ");
            }
            NativeModel.FilterBuilder.Append(filterString);
        }

        protected virtual void Visit(LucinqQuery query, StringBuilder stringBuilder = null, bool omitLeadingOperator = false)
        {
            if (stringBuilder == null)
            {
                stringBuilder = NativeModel.QueryBuilder;
            }
            if (query is LucinqQueryRoot queryRoot)
            {
                VisitPrimary(queryRoot, stringBuilder);
            }
            else if (query is LucinqAndQuery andQuery)
            {
                VisitAnd(andQuery, stringBuilder, omitLeadingOperator);
            }
            else if (query is LucinqOrQuery orQuery)
            {
                VisitOr(orQuery, stringBuilder, omitLeadingOperator);
            }

            else if (query is LucinqFuzzyQuery fuzzyQuery)
            {
                VisitFuzzy(fuzzyQuery, stringBuilder);
            }
            else if (query is LucinqPhraseQuery phraseQuery)
            {
                VisitPhrase(phraseQuery, stringBuilder);
            }
            else if (query is LucinqWildcardQuery wildcardQuery)
            {
                VisitWildcard(wildcardQuery, stringBuilder);
            }
            else if (query is LucinqPrefixQuery prefixQuery)
            {
                VisitPrefix(prefixQuery, stringBuilder);
            }
            else if (query is LucinqKeywordQuery keyworkQuery)
            {
                VisitKeyword(keyworkQuery, stringBuilder);
            }
            else if (query is LucinqTermQuery termQuery)
            {
                VisitTerm(termQuery, stringBuilder);
            }
            else if (query is LucinqRangeQuery<double> doubleQuery)
            {
                VisitDoubleRange(doubleQuery, stringBuilder);
            }
            else if (query is LucinqRangeQuery<int> intQuery)
            {
                VisitIntRange(intQuery, stringBuilder);
            }
            else if (query is LucinqRangeQuery<long> longQuery)
            {
                VisitLongRange(longQuery, stringBuilder);
            }
            else if (query is LucinqRangeQuery<string> termRangeQuery)
            {
                VisitTermRange(termRangeQuery, stringBuilder);
            }


        }

        protected virtual void VisitTermRange(LucinqRangeQuery<string> query, StringBuilder stringBuilder)
        {
            string lowerOperator = query.IncludeMin ? "[" : "{";
            string lower = !String.IsNullOrEmpty(query.Lower) ? $"{lowerOperator}{query.Lower}" : String.Empty;

            string upperOperator = query.IncludeMin ? "]" : "}";
            string upper = !String.IsNullOrEmpty(query.Upper) ? $"{query.Upper}{upperOperator}" : String.Empty;


            string value = $"{query.Field}:{lower} TO {upper}";

            NativeModel.FilterBuilder.Append($"{value}");
        }

        protected virtual void VisitIntRange(LucinqRangeQuery<int> query, StringBuilder stringBuilder)
        {
            string lowerOperator = query.IncludeMin ? "[" : "{";
            string lower = !String.IsNullOrEmpty(query.Lower.ToString()) ? $"{lowerOperator}{query.Lower}" : String.Empty;

            string upperOperator = query.IncludeMin ? "]" : "}";
            string upper = !String.IsNullOrEmpty(query.Upper.ToString()) ? $"{query.Upper}{upperOperator}" : String.Empty;


            string value = $"{query.Field}:{lower} TO {upper}";

            NativeModel.FilterBuilder.Append(value);
        }

        protected virtual void VisitLongRange(LucinqRangeQuery<long> query, StringBuilder stringBuilder)
        {
            string lowerOperator = query.IncludeMin ? "[" : "{";
            string lower = !String.IsNullOrEmpty(query.Lower.ToString()) ? $"{lowerOperator}{query.Lower}" : String.Empty;

            string upperOperator = query.IncludeMin ? "]" : "}";
            string upper = !String.IsNullOrEmpty(query.Upper.ToString()) ? $"{query.Upper}{upperOperator}" : String.Empty;


            string value = $"{query.Field}:{lower} TO {upper}";

            NativeModel.FilterBuilder.Append(value);
        }

        protected virtual void VisitDoubleRange(LucinqRangeQuery<double> query, StringBuilder stringBuilder)
        {
            string lowerOperator = query.IncludeMin ? "[" : "{";
            string lower = !String.IsNullOrEmpty(query.Lower.ToString()) ? $"{lowerOperator}{query.Lower}" : String.Empty;

            string upperOperator = query.IncludeMin ? "]" : "}";
            string upper = !String.IsNullOrEmpty(query.Upper.ToString()) ? $"{query.Upper}{upperOperator}" : String.Empty;


            string value = $"{query.Field}:{lower} TO {upper}";

            NativeModel.FilterBuilder.Append(value);
        }

        protected virtual void VisitKeyword(LucinqKeywordQuery query, StringBuilder stringBuilder)
        {
            stringBuilder.Append(GetTermQueryString(query, true, stringBuilder));
        }

        protected virtual void VisitPrefix(LucinqPrefixQuery query, StringBuilder stringBuilder)
        {
            stringBuilder.Append(GetTermQueryString(query, false, stringBuilder) + "*");
        }

        protected virtual void VisitWildcard(LucinqWildcardQuery query, StringBuilder stringBuilder)
        {
            stringBuilder.Append(GetTermQueryString(query, false, stringBuilder));
        }

        protected virtual void VisitTerm(LucinqTermQuery query, StringBuilder stringBuilder)
        {
            stringBuilder.Append(GetTermQueryString(query, false, stringBuilder));
        }

        private string GetTermQueryString(LucinqTermQuery query, bool quoteValue, StringBuilder stringBuilder, bool fuzzy = false)
        {
            string returnString = null;
            if (stringBuilder.Length > 0)
            {
                returnString += " ";
            }

            string occurrence = String.Empty;

            switch (query.Matches)
            {
                case Matches.Always:
                case Matches.NotSet:
                    occurrence += "+";
                    break;
                case Matches.Never:
                    occurrence += "-";
                    break;
            }

            string value = quoteValue ? $"\"{query.SearchTerm.Value}\"" : query.SearchTerm.Value;
            if (fuzzy)
            {
                value += "~";
            }

            string boostString = query.Boost.HasValue ? $"~{query.Boost.Value}" : String.Empty;
            returnString += $"{occurrence}{query.SearchTerm.Field}:{value}{boostString}";
            return returnString;
        }

        protected virtual void VisitGroup(LucinqGroupQuery query, StringBuilder stringBuilder, bool omitLeadingOperator = false)
        {



            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(" ");

                if (query.Matches == Matches.Always)
                {
                    stringBuilder.Append("AND ");
                }
                else
                {
                    stringBuilder.Append("OR ");
                }
            }

            StringBuilder builder = new StringBuilder();

            var first = true;
            foreach (var subQuery in query.Queries)
            {

                Visit(subQuery, builder);
                first = false;
            }

            stringBuilder.Append(builder);
        }

        protected virtual void VisitPrimary(LucinqGroupQuery query, StringBuilder stringBuilder)
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(" ");
            }

            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (var subQuery in query.Queries)
            {
                Visit(subQuery, builder, first);
                first = false;

            }

            stringBuilder.Append(builder);
        }

        protected virtual void VisitAnd(LucinqAndQuery query, StringBuilder stringBuilder, bool omitLeadingOperator = false)
        {
            VisitGroupQuery(query, stringBuilder, omitLeadingOperator, "AND");
        }

        protected virtual void VisitOr(LucinqOrQuery query, StringBuilder stringBuilder, bool omitLeadingOperator = false)
        {
            VisitGroupQuery(query, stringBuilder, omitLeadingOperator, "OR");
        }

        private void VisitGroupQuery(LucinqGroupQuery query, StringBuilder stringBuilder, bool omitLeadingOperator,
            string groupSeperator)
        {
            if (query.Queries.Count == 0 || query.Queries.All(IsRangeQuery))
            {
                return;
            }

            StringBuilder builder = new StringBuilder();
            bool first = true;

            if (stringBuilder.Length > 0)
            {
                builder.Append(" ");
            }

            if (!omitLeadingOperator)
            {
                if (query.Matches == Matches.Always)
                {
                    builder.Append("AND ");
                }
                else
                {
                    builder.Append("OR ");
                }
            }

            if (query.Queries.Count > 1)
            {
                builder.Append("(");
            }

            foreach (var subQuery in query.Queries)
            {
                if (IsRangeQuery(subQuery))
                {
                    continue;
                }

                if (!first && !IsGroupQuery(subQuery))
                {
                    string subgroupSeperator;
                    if (subQuery.Matches == Matches.Always)
                    {
                        subgroupSeperator = "AND";
                    }
                    else
                    {
                        subgroupSeperator = "OR";
                    }
                    builder.Append($" {subgroupSeperator}");
                }

                Visit(subQuery, builder, first);
                first = false;
            }

            if (query.Queries.Count > 1)
            {
                builder.Append(")");
            }

            stringBuilder.Append(builder);
        }

        protected bool IsRangeQuery(LucinqQuery query)
        {
            Type rangeQueryType = typeof(LucinqRangeQuery<>);
            Type queryType = query.GetType();
            return queryType.IsGenericType && rangeQueryType.IsAssignableFrom(queryType.GetGenericTypeDefinition());
        }

        private static bool IsGroupQuery(LucinqQuery subQuery)
        {
            return subQuery is LucinqAndQuery || subQuery is LucinqOrQuery;
        }


        protected virtual void VisitFuzzy(LucinqFuzzyQuery query, StringBuilder stringBuilder)
        {
            stringBuilder.Append(GetTermQueryString(query, false, stringBuilder, true));
        }

        protected virtual void VisitPhrase(LucinqPhraseQuery query, StringBuilder stringBuilder)
        {
            string returnString = null;
            if (stringBuilder.Length > 0)
            {
                returnString += " ";
            }

            string occurrence = String.Empty;

            switch (query.Matches)
            {
                case Matches.Always:
                case Matches.NotSet:
                    occurrence += "+";
                    break;
                case Matches.Never:
                    occurrence += "-";
                    break;
            }

            bool first = true;
            foreach (var term in query.LucinqTerms)
            {
                if (!first)
                {
                    returnString += " ";
                }

                string value = String.Empty;
                value += $"\"{term.Value}\"";

                string boostString = query.Boost.HasValue ? $"^{query.Boost.Value}" : String.Empty;
                returnString += $"{occurrence}{term.Field}:{value}~{query.Slop}{boostString}";
                first = false;
            }

            stringBuilder.Append(returnString);
        }
    }
}
