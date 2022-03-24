using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucinq.Core.Enums;
using Lucinq.Core.Querying;
using Lucinq.Lucene30.Adapters;
using Lucinq.Lucene30.Extensions;
using NUnit.Framework;
using Version = Lucene.Net.Util.Version;

namespace Lucinq.Lucene30.UnitTests.UnitTests
{
	[TestFixture]
	public class EquivalencyTests
	{
		#region [ Terms Tests ]

		[Test]
		public void CaseInsensitiveMandatoryTerm()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value"));
		    LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void BoostedCaseInsensitiveMandatoryTerm()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Matches.Always.GetLuceneOccurance());
			termQuery.Boost = 10;
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value", boost:10));
		    LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveMandatoryTerm()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "Value");
			TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value", caseSensitive:true));
		    LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void QueryCaseSensitiveMandatoryTerm()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "Value");
			TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
			builder.Setup(x => x.Term("_name", "Value"));
		    LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseInsensitiveNonMandatoryTerm()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Matches.Sometimes.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value", Matches.Sometimes));
		    LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveNonMandatoryTerm()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "Value");
			TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Matches.Sometimes.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value", Matches.Sometimes, caseSensitive: true));
		    LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

        [Test]
        public void CaseInsensitiveNotTerm()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            Term term = new Term("_name", "value");
            TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Matches.Never.GetLuceneOccurance());
            string queryString = originalQuery.ToString();


            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.Term("_name", "Value", Matches.Never));
            LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        [Test]
        public void CaseSensitiveNotTerm()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            Term term = new Term("_name", "Value");
            TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Matches.Never.GetLuceneOccurance());
            string queryString = originalQuery.ToString();


            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.Term("_name", "Value", Matches.Never, caseSensitive: true));
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

		#endregion

		#region [ Lucene Tests ]

        // todo: NM: Fix raw
        /*
		[Test]
		public void AddLuceneApiQuery()
		{
			// shows you can add regular lucene queries to lucinq
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			TermQuery termQuery2 = new TermQuery(term);
            builder.Add(termQuery2, Matches.Always);
			LucinqQueryModel replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}
        */

		#endregion

		#region [ Phrase Tests ]

		[Test]
		public void CaseInsensitivePhrase()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			PhraseQuery phraseQuery = new PhraseQuery {Slop = 2};
		    phraseQuery.Add(term);
            originalQuery.Add(phraseQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
		    var terms = new[]
		    {
		        new KeyValuePair<string, string>("_name", "value")
		    };
			builder.Setup(x => x.Phrase(2, terms));
		    LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void BoostedCaseInsensitivePhrase()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			PhraseQuery phraseQuery = new PhraseQuery {Slop = 2};
		    phraseQuery.Add(term);
			phraseQuery.Boost = 10;
            originalQuery.Add(phraseQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
		    var terms = new[]
		    {
		        new KeyValuePair<string, string>("_name", "Value")
		    };

			builder.Setup(x => x.Phrase(2, terms, 10));
		    LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitivePhrase()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "Value");
			PhraseQuery phraseQuery = new PhraseQuery {Slop = 2};
		    phraseQuery.Add(term);
            originalQuery.Add(phraseQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
		    var terms = new[]
		    {
		        new KeyValuePair<string, string>("_name", "Value")
		    };

			builder.Setup(x => x.Phrase(2, terms));
		    LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void QueryCaseSensitivePhrase()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "Value");
			PhraseQuery phraseQuery = new PhraseQuery {Slop = 2};
		    phraseQuery.Add(term);
            originalQuery.Add(phraseQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
		    var terms = new[]
		    {
		        new KeyValuePair<string, string>("_name", "Value")
		    };

			builder.Setup(x => x.Phrase(2, terms));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		#endregion

		#region [ WildCard Tests ]

		[Test]
		public void CaseInsensitiveMandatoryWildCard()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value*");
			WildcardQuery wildcardQuery = new WildcardQuery(term);
            originalQuery.Add(wildcardQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.WildCard("_name", "Value*"));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveMandatoryWildCard()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "Value*");
			WildcardQuery wildcardQuery = new WildcardQuery(term);
            originalQuery.Add(wildcardQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.WildCard("_name", "Value*", caseSensitive:true));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void QueryCaseSensitiveMandatoryWildCard()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "Value*");
			WildcardQuery wildcardQuery = new WildcardQuery(term);
            originalQuery.Add(wildcardQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
			builder.Setup(x => x.WildCard("_name", "Value*"));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseInsensitiveNonMandatoryWildCard()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value*");
			WildcardQuery wildcardQuery = new WildcardQuery(term);
            originalQuery.Add(wildcardQuery, Matches.Sometimes.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.WildCard("_name", "Value*", Matches.Sometimes));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveNonMandatoryWildCard()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "Value*");
			WildcardQuery wildcardQuery = new WildcardQuery(term);
            originalQuery.Add(wildcardQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.WildCard("_name", "Value*", caseSensitive: true));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}


        [Test]
        public void CaseInsensitiveWildCards()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            BooleanQuery innerQuery = new BooleanQuery();
            Term term = new Term("_name", "value*");
            WildcardQuery wildcardQuery = new WildcardQuery(term);
            innerQuery.Add(wildcardQuery, Matches.Always.GetLuceneOccurance());

            Term term2 = new Term("_name", "value2*");
            WildcardQuery wildcardQuery2 = new WildcardQuery(term2);
            innerQuery.Add(wildcardQuery2, Matches.Always.GetLuceneOccurance());
            originalQuery.Add(innerQuery, Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.WildCards("_name", new []{"Value*", "Value2*"}));
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        #endregion

        #region [ Fuzzy Tests ]

        [Test]
        public void CaseInsensitiveMandatoryFuzzy()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            Term term = new Term("_name", "value*");
            FuzzyQuery fuzzyQuery = new FuzzyQuery(term, 0.9f);
            originalQuery.Add(fuzzyQuery, Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.Fuzzy("_name", "Value*", 0.9f));
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        [Test]
        public void CaseSensitiveMandatoryFuzzy()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            Term term = new Term("_name", "Value*");
            FuzzyQuery fuzzyQuery = new FuzzyQuery(term, 0.9f);
            originalQuery.Add(fuzzyQuery, Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.Fuzzy("_name", "Value*", 0.9f, caseSensitive: true));
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        [Test]
        public void QueryCaseSensitiveMandatoryFuzzy()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            Term term = new Term("_name", "Value*");
            FuzzyQuery wildcardQuery = new FuzzyQuery(term, 0.9f);
            originalQuery.Add(wildcardQuery, Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            QueryBuilder builder = new QueryBuilder { CaseSensitive = true };
            builder.Setup(x => x.Fuzzy("_name", "Value*", 0.9f));
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        [Test]
        public void CaseInsensitiveNonMandatoryFuzzy()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            Term term = new Term("_name", "value*");
            FuzzyQuery fuzzyQuery = new FuzzyQuery(term, 0.9f);
            originalQuery.Add(fuzzyQuery, Matches.Sometimes.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.Fuzzy("_name", "Value*", 0.9f, Matches.Sometimes));
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        [Test]
        public void CaseSensitiveNonMandatoryFuzzy()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            Term term = new Term("_name", "Value*");
            FuzzyQuery fuzzyQuery = new FuzzyQuery(term, 0.9f);
            originalQuery.Add(fuzzyQuery, Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.Fuzzy("_name", "Value*", 0.9f, caseSensitive: true));
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        #endregion

        #region [ Term Range Tests ]

        [Test]
		public void CaseInSensitiveTermRange()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			TermRangeQuery termRangeQuery = new TermRangeQuery("field", "lower", "upper", true, true);
            originalQuery.Add(termRangeQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.TermRange("field", "Lower", "Upper"));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveTermRange()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			TermRangeQuery termRangeQuery = new TermRangeQuery("field", "Lower", "Upper", true, true);
            originalQuery.Add(termRangeQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.TermRange("field", "Lower", "Upper", caseSensitive:true));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

        [Test]
        public void CaseInsensitiveTermRange()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            TermRangeQuery termRangeQuery = new TermRangeQuery("field", "lower", "upper", true, true);
            originalQuery.Add(termRangeQuery, Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.TermRange("field", "Lower", "Upper", caseSensitive: false));
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

		[Test]
		public void QueryCaseSensitiveTermRange()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			TermRangeQuery termRangeQuery = new TermRangeQuery("field", "Lower", "Upper", true, true);
            originalQuery.Add(termRangeQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
			builder.Setup(x => x.TermRange("field", "Lower", "Upper"));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		#endregion

		#region [ Keyword Tests ]

		[Test]
		public void CaseInsensitiveKeyword()
		{
			QueryBuilder builder = new QueryBuilder();

			BooleanQuery originalQuery = new BooleanQuery();
			QueryParser rawQueryParser = new QueryParser(Version.LUCENE_29, "_name", new KeywordAnalyzer());
            originalQuery.Add(rawQueryParser.Parse("value"), Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			builder.Setup(x => x.Keyword("_name", "Value"));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveKeyword()
		{
			QueryBuilder builder = new QueryBuilder();

			BooleanQuery originalQuery = new BooleanQuery();
			QueryParser rawQueryParser = new QueryParser(Version.LUCENE_29, "_name", new KeywordAnalyzer());
            originalQuery.Add(rawQueryParser.Parse("Value"), Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			builder.Setup(x => x.Keyword("_name", "Value", caseSensitive:true));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}


		[Test]
		public void CaseInsensitiveNonMandatoryKeyword()
		{
			QueryBuilder builder = new QueryBuilder();

			BooleanQuery originalQuery = new BooleanQuery();
			QueryParser rawQueryParser = new QueryParser(Version.LUCENE_29, "_name", new KeywordAnalyzer());
            originalQuery.Add(rawQueryParser.Parse("value"), Matches.Sometimes.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			builder.Setup(x => x.Keyword("_name", "Value", Matches.Sometimes));
			LucinqQueryModel replacementQuery = builder.Build();
            LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveNonMandatoryKeyword()
		{
			QueryBuilder builder = new QueryBuilder();

			BooleanQuery originalQuery = new BooleanQuery();
			QueryParser rawQueryParser = new QueryParser(Version.LUCENE_29, "_name", new KeywordAnalyzer());
            originalQuery.Add(rawQueryParser.Parse("Value"), Matches.Sometimes.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			builder.Setup(x => x.Keyword("_name", "Value", Matches.Sometimes, caseSensitive: true));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

        [Test]
        public void CaseInsensitiveKeywords()
        {
            QueryBuilder builder = new QueryBuilder();

            BooleanQuery originalQuery = new BooleanQuery();
            BooleanQuery innerQuery = new BooleanQuery();
            QueryParser rawQueryParser = new QueryParser(Version.LUCENE_29, "_name", new KeywordAnalyzer());
            innerQuery.Add(rawQueryParser.Parse("value"), Matches.Always.GetLuceneOccurance());
            innerQuery.Add(rawQueryParser.Parse("value2"), Matches.Always.GetLuceneOccurance());
            innerQuery.Add(rawQueryParser.Parse("value3"), Matches.Always.GetLuceneOccurance());
            originalQuery.Add(innerQuery, Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();
            originalQuery.Add(rawQueryParser.Parse("value2"), Matches.Always.GetLuceneOccurance());

            builder.Setup(x => x.Keywords("_name", new []{"Value", "Value2", "Value3"}));
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

		#endregion

		#region [ Numeric Range Tests ]

		[Test]
		public void IntegerRange()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			NumericRangeQuery<int> numericRangeQuery = NumericRangeQuery.NewIntRange("field", 1, 0, 10, true, true);
            originalQuery.Add(numericRangeQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.NumericRange("field", 0, 10));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void DoubleRange()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			NumericRangeQuery<double> numericRangeQuery = NumericRangeQuery.NewDoubleRange("field", 1, 0d, 10d, true, true);
            originalQuery.Add(numericRangeQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.NumericRange("field", 0d, 10d));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void LongRange()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			NumericRangeQuery<long> numericRangeQuery = NumericRangeQuery.NewLongRange("field", 1, 0L, 10L, true, true);
            originalQuery.Add(numericRangeQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.NumericRange("field", 0L, 10L));
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		#endregion

		#region [ Grouping Tests ]

		[Test]
		public void Or()
		{
			BooleanQuery originalQuery = new BooleanQuery();

			Term term = new Term("_name", "value1");
			TermQuery termQuery1 = new TermQuery(term);
            originalQuery.Add(termQuery1, Matches.Sometimes.GetLuceneOccurance());

			Term term2 = new Term("_name", "value2");
			TermQuery termQuery2 = new TermQuery(term2);
            originalQuery.Add(termQuery2, Matches.Sometimes.GetLuceneOccurance());

			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder{DefaultChildrenOccur = Matches.Sometimes};
			builder.Setup
				(
					x => x.Term("_name", "value1"),
					x => x.Term("_name", "value2")
				);
			LucinqQueryModel replacementQuery1 = builder.Build();
		    LuceneAdapter adapter = new LuceneAdapter();

		    string newQueryString1 = adapter.Adapt(replacementQuery1).Query.ToString();

            Assert.AreEqual(queryString, newQueryString1);

			QueryBuilder builder2 = new QueryBuilder();
			builder2.Setup
				(
					x => x.Term("_name", "value1", Matches.Sometimes),
					x => x.Term("_name", "value2", Matches.Sometimes)
				);
			LucinqQueryModel replacementQuery2 = builder2.Build();
		    string newQueryString2 = adapter.Adapt(replacementQuery2).Query.ToString();

            Assert.AreEqual(queryString, newQueryString2);

			QueryBuilder builder3 = new QueryBuilder();
			builder3.Setup
				(
					x => x.Term("_name", "value1"),
					x => x.Term("_name", "value2")
				);
			LucinqQueryModel replacementQuery3 = builder3.Build();
		    string newQueryString3 = adapter.Adapt(replacementQuery3).Query.ToString();

			Assert.AreNotEqual(queryString, newQueryString3);
			
			Console.Write(queryString);
		}

        [Test]
	    public void OptionalOr()
	    {
            BooleanQuery originalQuery = new BooleanQuery();

            BooleanQuery innerQuery = new BooleanQuery();

            Term term = new Term("_name", "value1");
            TermQuery termQuery1 = new TermQuery(term);
            innerQuery.Add(termQuery1, Matches.Sometimes.GetLuceneOccurance());

            Term term2 = new Term("_name", "value2");
            TermQuery termQuery2 = new TermQuery(term2);
            innerQuery.Add(termQuery2, Matches.Sometimes.GetLuceneOccurance());
            originalQuery.Add(innerQuery, Matches.Sometimes.GetLuceneOccurance());

            string queryString = originalQuery.ToString();

            QueryBuilder builder = new QueryBuilder { DefaultChildrenOccur = Matches.Sometimes };
            builder.Or
                (
                    Matches.Sometimes,
                    x => x.Term("_name", "value1"),
                    x => x.Term("_name", "value2")
                );
            LucinqQueryModel replacementQuery = builder.Build();

	        LuceneAdapter adapter = new LuceneAdapter();
	        string newQueryString1 = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString1);
	    }

		[Test]
		public void OrExtension()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			BooleanQuery innerQuery = new BooleanQuery();
			
			Term term = new Term("_name", "value1");
			TermQuery termQuery1 = new TermQuery(term);
            innerQuery.Add(termQuery1, Matches.Sometimes.GetLuceneOccurance());

			Term term2 = new Term("_name", "value2");
			TermQuery termQuery2 = new TermQuery(term2);
            innerQuery.Add(termQuery2, Matches.Sometimes.GetLuceneOccurance());

            originalQuery.Add(innerQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			var builder2 = builder.Or
				(
					x => x.Term("_name", "value1"),
					x => x.Term("_name", "value2")
				);

			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

        [Test]
        public void CreateOrExtension()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            BooleanQuery innerQuery = new BooleanQuery();

            Term term = new Term("_name", "value1");
            TermQuery termQuery1 = new TermQuery(term);
            innerQuery.Add(termQuery1, Matches.Sometimes.GetLuceneOccurance());

            Term term2 = new Term("_name", "value2");
            TermQuery termQuery2 = new TermQuery(term2);
            innerQuery.Add(termQuery2, Matches.Sometimes.GetLuceneOccurance());

            originalQuery.Add(innerQuery, Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            QueryBuilder builder = new QueryBuilder();
            var group = builder.CreateOrGroup
                (
                    x => x.Term("_name", "value1"),
                    x => x.Term("_name", "value2")
                );
            Assert.AreNotEqual(group, builder);
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

		[Test]
		public void AndExtension()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			BooleanQuery innerQuery = new BooleanQuery();

			Term term = new Term("_name", "value1");
			TermQuery termQuery1 = new TermQuery(term);
            innerQuery.Add(termQuery1, Matches.Always.GetLuceneOccurance());

			Term term2 = new Term("_name", "value2");
			TermQuery termQuery2 = new TermQuery(term2);
            innerQuery.Add(termQuery2, Matches.Always.GetLuceneOccurance());

            originalQuery.Add(innerQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			var builder2 = builder.And
				(
					x => x.Term("_name", "value1"),
					x => x.Term("_name", "value2")
				);

			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

        [Test]
        public void CreateAndExtension()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            BooleanQuery innerQuery = new BooleanQuery();

            Term term = new Term("_name", "value1");
            TermQuery termQuery1 = new TermQuery(term);
            innerQuery.Add(termQuery1, Matches.Always.GetLuceneOccurance());

            Term term2 = new Term("_name", "value2");
            TermQuery termQuery2 = new TermQuery(term2);
            innerQuery.Add(termQuery2, Matches.Always.GetLuceneOccurance());

            originalQuery.Add(innerQuery, Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            QueryBuilder builder = new QueryBuilder();
            var builder2 = builder.CreateAndGroup
                (
                    x => x.Term("_name", "value1"),
                    x => x.Term("_name", "value2")
                );

            Assert.AreNotEqual(builder2, builder);
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        [Test]
        public void OptionalAndExtension()
        {
            BooleanQuery originalQuery = new BooleanQuery();
            BooleanQuery innerQuery = new BooleanQuery();

            Term term = new Term("_name", "value1");
            TermQuery termQuery1 = new TermQuery(term);
            innerQuery.Add(termQuery1, Matches.Always.GetLuceneOccurance());

            Term term2 = new Term("_name", "value2");
            TermQuery termQuery2 = new TermQuery(term2);
            innerQuery.Add(termQuery2, Matches.Always.GetLuceneOccurance());

            originalQuery.Add(innerQuery, Matches.Sometimes.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            QueryBuilder builder = new QueryBuilder();
            builder.And
                (
                    Matches.Sometimes,
                    x => x.Term("_name", "value1"),
                    x => x.Term("_name", "value2")
                );
            LucinqQueryModel replacementQuery = builder.Build();

            LuceneAdapter adapter = new LuceneAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

		#endregion

		#region [ Composite Tests ]

		[Test]
		public void CompositeTermPhraseWildcardTests()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			TermQuery termQuery = new TermQuery(term);
            originalQuery.Add(termQuery, Matches.Always.GetLuceneOccurance());
			PhraseQuery phraseQuery = new PhraseQuery();
			Term phraseTerm = new Term("_name", "phrase");
		    phraseQuery.Slop = 2;
			phraseQuery.Add(phraseTerm);
            originalQuery.Add(phraseQuery, Matches.Always.GetLuceneOccurance());

			Term wildcardTerm = new Term("_name", "*wildcard*");
			WildcardQuery wildcardQuery = new WildcardQuery(wildcardTerm);
            originalQuery.Add(wildcardQuery, Matches.Sometimes.GetLuceneOccurance());

			string queryString = originalQuery.ToString();



			QueryBuilder builder = new QueryBuilder();
			builder.Setup
				(
					x => x.Term("_name", "value"),
					x => x.Phrase(2, new []{new KeyValuePair<string, string>("_name", "phrase") }),
					x => x.WildCard("_name", "*wildcard*", Matches.Sometimes)
				);
			LucinqQueryModel replacementQuery = builder.Build();

		    LuceneAdapter adapter = new LuceneAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).Query.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		#endregion

        #region [ Constructor Tests ]

        [Test]
	    public void SetupConstructorTest()
	    {
            QueryBuilder setupBuilder = new QueryBuilder();
            setupBuilder.Setup(x => x.Term("_name", "Value"));
	        LucinqQueryModel setupQuery = setupBuilder.Build();
            string setupQueryString = setupQuery.ToString();

            QueryBuilder constructorBuilder = new QueryBuilder(x => x.Term("_name", "Value"));
	        LucinqQueryModel constructorQuery = constructorBuilder.Build();
            string constructorQueryString = constructorQuery.ToString();

            Assert.AreEqual(setupQueryString, constructorQueryString);
        }

        #endregion

        #region [ Raw Tests ]
        
        // todo: NM: Fix Raw
        /*
        [Test]
	    public void RawWithAnalyzer()
	    {
            QueryBuilder builder = new QueryBuilder();

            BooleanQuery originalQuery = new BooleanQuery();
            QueryParser rawQueryParser = new QueryParser(Version.LUCENE_29, "_name", builder.KeywordAnalyzer);
            originalQuery.Add(rawQueryParser.Parse("value"), Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            KeywordAnalyzer analyzer = new KeywordAnalyzer();
            builder.Setup(x => x.Raw("_name", "value", Matches.Always, analyzer: analyzer));
            LucinqQueryModel replacementQuery = builder.Build();
            string newQueryString = replacementQuery.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
	    }

        [Test]
        public void RawWithoutAnalyzer()
        {
            QueryBuilder builder = new QueryBuilder();

            BooleanQuery originalQuery = new BooleanQuery();
            TermQuery termQuery = new TermQuery(new Term("_name", "value"));
            originalQuery.Add(termQuery, Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();

            builder.Setup(x => x.Raw("_name", "value", Matches.Always));
            LucinqQueryModel replacementQuery = builder.Build();
            string newQueryString = replacementQuery.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }
        */

        #endregion
    }
}
