using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Lucinq.Core.Querying;
using Lucinq.Lucene30.Extensions;

namespace Lucinq.Lucene30.Adapters
{
    public class LuceneAdapter : IProviderAdapter<LuceneModel>
    {
        protected LuceneModel NativeModel { get; set; }

        public virtual LuceneModel Adapt(LucinqQueryModel model)
        {
            NativeModel = new LuceneModel();
            Visit(model.Query, NativeModel.Query);
            Visit(model.Sort);
            Visit(model.Filter);
            return NativeModel;
        }

        protected virtual void Visit(LucinqSort sort)
        {
            if (sort == null || sort.SortFields.Length == 0)
            {
                return;
            }

            NativeModel.Sort = new Sort(sort.SortFields.Select(x => new SortField(x.FieldName, x.SortType, x.SortDescending)).ToArray());
        }

        protected virtual void Visit(LucinqFilter filter)
        {

        }

        protected virtual void Visit(LucinqQuery query, BooleanQuery booleanQuery = null)
        {
            if (query is LucinqQueryRoot queryRoot)
            {
                VisitPrimary(queryRoot);
            }
            else if (query is LucinqGroupQuery groupQuery)
            {
                VisitGroup(groupQuery, booleanQuery);
            }
            else if (query is LucinqFuzzyQuery fuzzyQuery)
            {
                VisitFuzzy(fuzzyQuery, booleanQuery);
            }
            else if (query is LucinqPhraseQuery phraseQuery)
            {
                VisitPhrase(phraseQuery, booleanQuery);
            }
            else if (query is LucinqWildcardQuery wildcardQuery)
            {
                VisitWildcard(wildcardQuery, booleanQuery);
            }
            else if (query is LucinqPrefixQuery prefixQuery)
            {
                VisitPrefix(prefixQuery, booleanQuery);
            }
            else if (query is LucinqKeywordQuery keyworkQuery)
            {
                VisitKeyword(keyworkQuery, booleanQuery);
            }
            else if (query is LucinqTermQuery termQuery)
            {
                VisitTerm(termQuery, booleanQuery);
            }
            else if (query is LucinqRangeQuery<double> doubleQuery)
            {
                VisitDoubleRange(doubleQuery, booleanQuery);
            }
            else if (query is LucinqRangeQuery<int> intQuery)
            {
                VisitIntRange(intQuery, booleanQuery);
            }
            else if (query is LucinqRangeQuery<long> longQuery)
            {
                VisitLongRange(longQuery, booleanQuery);
            }
            else if (query is LucinqRangeQuery<string> termRangeQuery)
            {
                VisitTermRange(termRangeQuery, booleanQuery);
            }
        }

        protected virtual void VisitTermRange(LucinqRangeQuery<string> query, BooleanQuery booleanQuery)
        {
            TermRangeQuery nativeQuery = new TermRangeQuery(query.Field, query.Lower, query.Upper, query.IncludeMin, query.IncludeMax);
            AddQuery(query, booleanQuery, nativeQuery);
        }

        protected virtual void VisitIntRange(LucinqRangeQuery<int> query, BooleanQuery booleanQuery)
        {
            NumericRangeQuery<int> nativeQuery = NumericRangeQuery.NewIntRange(query.Field, query.PrecisionStep ?? 1, query.Lower, query.Upper, query.IncludeMin, query.IncludeMax);
            AddQuery(query, booleanQuery, nativeQuery);
        }

        protected virtual void VisitLongRange(LucinqRangeQuery<long> query, BooleanQuery booleanQuery)
        {
            NumericRangeQuery<long> nativeQuery = NumericRangeQuery.NewLongRange(query.Field, query.PrecisionStep ?? 1, query.Lower, query.Upper, query.IncludeMin, query.IncludeMax);
            AddQuery(query, booleanQuery, nativeQuery);
        }

        protected virtual void VisitDoubleRange(LucinqRangeQuery<double> query, BooleanQuery booleanQuery)
        {
            NumericRangeQuery<double> nativeQuery = NumericRangeQuery.NewDoubleRange(query.Field, query.PrecisionStep ?? 1, query.Lower, query.Upper, query.IncludeMin, query.IncludeMax);
            AddQuery(query, booleanQuery, nativeQuery);
        }

        protected virtual void VisitKeyword(LucinqKeywordQuery query, BooleanQuery booleanQuery)
        {
            Raw(query, booleanQuery, new KeywordAnalyzer());
        }

        protected virtual void VisitPrefix(LucinqPrefixQuery query, BooleanQuery booleanQuery)
        {
            Term term = new Term(query.SearchTerm.Field, query.SearchTerm.Value);
            PrefixQuery nativeQuery = new PrefixQuery(term);

            AddQuery(query, booleanQuery, nativeQuery);
        }

        protected virtual void VisitWildcard(LucinqWildcardQuery query, BooleanQuery booleanQuery)
        {
            Term term = new Term(query.SearchTerm.Field, query.SearchTerm.Value);
            WildcardQuery nativeQuery = new WildcardQuery(term);

            AddQuery(query, booleanQuery, nativeQuery);
        }

        protected virtual void VisitTerm(LucinqTermQuery query, BooleanQuery booleanQuery)
        {
            Term term = new Term(query.SearchTerm.Field, query.SearchTerm.Value);
            TermQuery nativeQuery = new TermQuery(term);

            AddQuery(query, booleanQuery, nativeQuery);
        }

        protected virtual void VisitPrimary(LucinqGroupQuery query)
        {
            foreach (var subQuery in query.Queries)
            {
                Visit(subQuery, NativeModel.Query);
            }
        }

        protected virtual void VisitGroup(LucinqGroupQuery query, BooleanQuery booleanQuery)
        {
            BooleanQuery groupQuery = new BooleanQuery();
            foreach (var subQuery in query.Queries)
            {
                Visit(subQuery, groupQuery);
            }

            booleanQuery.Add(groupQuery, query.Matches.GetLuceneOccurance());
        }

        protected virtual void VisitFuzzy(LucinqFuzzyQuery query, BooleanQuery booleanQuery)
        {
            Term term = new Term(query.SearchTerm.Field, query.SearchTerm.Value);
            FuzzyQuery nativeQuery = new FuzzyQuery(term, query.MinSimilarity);

            AddQuery(query, booleanQuery, nativeQuery);
        }

        protected virtual void VisitPhrase(LucinqPhraseQuery query, BooleanQuery booleanQuery)
        {
            PhraseQuery nativeQuery = new PhraseQuery();
            foreach (var lucinqTerm in query.LucinqTerms)
            {
                Term term = new Term(lucinqTerm.Field, lucinqTerm.Value);
                nativeQuery.Add(term);
            }

            nativeQuery.Slop = query.Slop;

            AddQuery(query, booleanQuery, nativeQuery);
        }

        protected virtual void Raw(LucinqTermQuery query, BooleanQuery booleanQuery, Analyzer analyzer = null)
        {
            if (analyzer == null)
            {
                analyzer = new StandardAnalyzer(Version.LUCENE_30);
            }
            QueryParser queryParser = new QueryParser(Version.LUCENE_30, query.SearchTerm.Field, analyzer);
            Query nativeQuery = queryParser.Parse(query.SearchTerm.Value);

            AddQuery(query, booleanQuery, nativeQuery);
        }

        protected virtual void AddQuery(LucinqQuery query, BooleanQuery booleanQuery, Query nativeQuery)
        {
            if (query.Boost.HasValue)
            {
                nativeQuery.Boost = query.Boost.Value;
            }

            booleanQuery.Add(nativeQuery, query.Matches.GetLuceneOccurance());
        }
    }
}
