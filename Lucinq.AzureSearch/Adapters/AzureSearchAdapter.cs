using System;
using System.Collections.Generic;
using System.Text;
using Lucinq.AzureSearch.Querying;
using Lucinq.Core.Adapters;
using Lucinq.Core.Enums;
using Lucinq.Core.Querying;
using Microsoft.Azure.Search.Models;

namespace Lucinq.AzureSearch.Adapters
{
    using System.Linq;

    public class AzureSearchAdapter : IProviderAdapter<AzureSearchModel>
    {
        protected AzureSearchModel NativeModel { get; set; }

        public virtual AzureSearchModel Adapt(LucinqQueryModel model)
        {
            NativeModel = new AzureSearchModel();
            NativeModel.SearchParameters.QueryType = QueryType.Full;
            NativeModel.SearchParameters.SearchMode = SearchMode.All;
            Visit(model.Query);
            Visit(model.Sort);
            Visit(model.Filter);
            NativeModel.SearchParameters.Filter = NativeModel.FilterBuilder.ToString();
            return NativeModel;
        }

        protected virtual void Visit(LucinqSort sort)
        {
            if (sort?.SortFields == null || sort.SortFields.Length == 0)
            {
                return;
            }

            NativeModel.SearchParameters.OrderBy = new List<string>();
            foreach (var sortField in sort.SortFields)
            {
                string orderBy = sortField.FieldName;
                if (sortField.SortDescending)
                {
                    orderBy += " desc";
                }

                NativeModel.SearchParameters.OrderBy.Add(orderBy);
            }
        }

        protected virtual void Visit(LucinqFilter filter)
        {
            if (filter == null)
            {
                return;
            }

            string comparator;
            switch (filter.Comparator)
            {
                case Comparator.Equals:
                    comparator = "eq";
                    break;
                case Comparator.NotEquals:
                    comparator = "ne";
                    break;
                case Comparator.GreaterThan:
                    comparator = "gt";
                    break;
                case Comparator.GreaterThanEquals:
                    comparator = "ge";
                    break;
                case Comparator.LessThan:
                    comparator = "lt";
                    break;
                case Comparator.LessThanEquals:
                    comparator = "le";
                    break;
                default:
                    comparator = "eq";
                    break;
            }
            var filterString = $"{filter.Field} {comparator} {filter.Value}";

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
            else if (query is LucinqOrQuery orQuery)
            {
                VisitOr(orQuery, stringBuilder, omitLeadingOperator);
            }
            else if (query is LucinqAndQuery andQuery)
            {
                VisitAnd(andQuery, stringBuilder, omitLeadingOperator);
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
            string lowerOperator = query.IncludeMin ? "ge" : "gt";
            string lower = !String.IsNullOrEmpty(query.Lower) ? $"{query.Field} {lowerOperator} {query.Lower}" : String.Empty;

            string upperOperator = query.IncludeMin ? "le" : "lt";
            string upper = !String.IsNullOrEmpty(query.Upper) ? $"{query.Field} {upperOperator} {query.Upper}" : String.Empty;

            NativeModel.FilterBuilder.Append($"{lower} and {upper}");
        }

        protected virtual void VisitIntRange(LucinqRangeQuery<int> query, StringBuilder stringBuilder)
        {
            string lowerOperator = query.IncludeMin ? "ge" : "gt";
            string lower = !String.IsNullOrEmpty(query.Lower.ToString()) ? $"{query.Field} {lowerOperator} {query.Lower.ToString()}" : String.Empty;

            string upperOperator = query.IncludeMin ? "le" : "lt";
            string upper = !String.IsNullOrEmpty(query.Upper.ToString()) ? $"{query.Field} {upperOperator} {query.Upper.ToString()}" : String.Empty;

            NativeModel.FilterBuilder.Append($"{lower} and {upper}");
        }

        protected virtual void VisitLongRange(LucinqRangeQuery<long> query, StringBuilder stringBuilder)
        {
            string lowerOperator = query.IncludeMin ? "ge" : "gt";
            string lower = !String.IsNullOrEmpty(query.Lower.ToString()) ? $"{query.Field} {lowerOperator} {query.Lower.ToString()}" : String.Empty;

            string upperOperator = query.IncludeMin ? "le" : "lt";
            string upper = !String.IsNullOrEmpty(query.Upper.ToString()) ? $"{query.Field} {upperOperator} {query.Upper.ToString()}" : String.Empty;

            NativeModel.FilterBuilder.Append($"{lower} and {upper}");
        }

        protected virtual void VisitDoubleRange(LucinqRangeQuery<double> query, StringBuilder stringBuilder)
        {
            string lowerOperator = query.IncludeMin ? "ge" : "gt";
            string lower = !String.IsNullOrEmpty(query.Lower.ToString()) ? $"{query.Field} {lowerOperator} {query.Lower.ToString()}" : String.Empty;

            string upperOperator = query.IncludeMin ? "le" : "lt";
            string upper = !String.IsNullOrEmpty(query.Upper.ToString()) ? $"{query.Field} {upperOperator} {query.Upper.ToString()}" : String.Empty;

            NativeModel.FilterBuilder.Append($"{lower} and {upper}");
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

        private string GetTermQueryString(LucinqTermQuery query, bool quoteValue, StringBuilder stringBuilder)
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

            string boostString = query.Boost.HasValue ? $"~{query.Boost.Value}" : String.Empty;
            returnString += $"{occurrence}{query.SearchTerm.Field}:{value}{boostString}";
            return returnString;
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

        private void VisitGroupQuery(LucinqGroupQuery query, StringBuilder stringBuilder, bool omitLeadingOperator, string groupSeperator)
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
                    builder.Append($" {groupSeperator}");
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

        private static bool IsGroupQuery(LucinqQuery subQuery)
        {
            return subQuery is LucinqAndQuery || subQuery is LucinqOrQuery;
        }

        protected virtual void VisitFuzzy(LucinqFuzzyQuery query, StringBuilder stringBuilder)
        {
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

        protected bool IsRangeQuery(LucinqQuery query)
        {
            Type rangeQueryType = typeof(LucinqRangeQuery<>);
            Type queryType = query.GetType();
            return queryType.IsGenericType && rangeQueryType.IsAssignableFrom(queryType.GetGenericTypeDefinition());
        }
    }
}
