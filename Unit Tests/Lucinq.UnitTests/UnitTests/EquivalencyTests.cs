using System;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucinq.Enums;
using Lucinq.Extensions;
using Lucinq.Querying;
using NUnit.Framework;
using Version = Lucene.Net.Util.Version;

namespace Lucinq.UnitTests.UnitTests
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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
            Query replacementQuery = builder.Build();
            string newQueryString = replacementQuery.ToString();

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
            Query replacementQuery = builder.Build();
            string newQueryString = replacementQuery.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

		#endregion

		#region [ Lucene Tests ]

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		#endregion

		#region [ Phrase Tests ]

		[Test]
		public void CaseInsensitivePhrase()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			PhraseQuery phraseQuery = new PhraseQuery();
			phraseQuery.Slop = 2;
			phraseQuery.Add(term);
            originalQuery.Add(phraseQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Phrase(2).AddTerm("_name", "Value"));
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void BoostedCaseInsensitivePhrase()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			PhraseQuery phraseQuery = new PhraseQuery();
		    phraseQuery.Slop = 2;
			phraseQuery.Add(term);
			phraseQuery.Boost = 10;
            originalQuery.Add(phraseQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Phrase(2, 10).AddTerm("_name", "Value"));
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitivePhrase()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "Value");
			PhraseQuery phraseQuery = new PhraseQuery();
			phraseQuery.Slop = 2;
			phraseQuery.Add(term);
            originalQuery.Add(phraseQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
			builder.Setup(x => x.Phrase(2).AddTerm("_name", "Value", true));
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void QueryCaseSensitivePhrase()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "Value");
			PhraseQuery phraseQuery = new PhraseQuery();
			phraseQuery.Slop = 2;
			phraseQuery.Add(term);
            originalQuery.Add(phraseQuery, Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
			builder.Setup(x => x.Phrase(2).AddTerm(x, "_name", "Value"));
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
            Query replacementQuery = builder.Build();
            string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
            Query replacementQuery = builder.Build();
            string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			QueryParser rawQueryParser = new QueryParser(Version.LUCENE_29, "_name", builder.KeywordAnalyzer);
            originalQuery.Add(rawQueryParser.Parse("value"), Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			builder.Setup(x => x.Keyword("_name", "Value"));
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveKeyword()
		{
			QueryBuilder builder = new QueryBuilder();

			BooleanQuery originalQuery = new BooleanQuery();
			QueryParser rawQueryParser = new QueryParser(Version.LUCENE_29, "_name", builder.KeywordAnalyzer);
            originalQuery.Add(rawQueryParser.Parse("Value"), Matches.Always.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			builder.Setup(x => x.Keyword("_name", "Value", caseSensitive:true));
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}


		[Test]
		public void CaseInsensitiveNonMandatoryKeyword()
		{
			QueryBuilder builder = new QueryBuilder();

			BooleanQuery originalQuery = new BooleanQuery();
			QueryParser rawQueryParser = new QueryParser(Version.LUCENE_29, "_name", builder.KeywordAnalyzer);
            originalQuery.Add(rawQueryParser.Parse("value"), Matches.Sometimes.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			builder.Setup(x => x.Keyword("_name", "Value", Matches.Sometimes));
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveNonMandatoryKeyword()
		{
			QueryBuilder builder = new QueryBuilder();

			BooleanQuery originalQuery = new BooleanQuery();
			QueryParser rawQueryParser = new QueryParser(Version.LUCENE_29, "_name", builder.KeywordAnalyzer);
            originalQuery.Add(rawQueryParser.Parse("Value"), Matches.Sometimes.GetLuceneOccurance());
			string queryString = originalQuery.ToString();


			builder.Setup(x => x.Keyword("_name", "Value", Matches.Sometimes, caseSensitive: true));
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

        [Test]
        public void CaseInsensitiveKeywords()
        {
            QueryBuilder builder = new QueryBuilder();

            BooleanQuery originalQuery = new BooleanQuery();
            BooleanQuery innerQuery = new BooleanQuery();
            QueryParser rawQueryParser = new QueryParser(Version.LUCENE_29, "_name", builder.KeywordAnalyzer);
            innerQuery.Add(rawQueryParser.Parse("value"), Matches.Always.GetLuceneOccurance());
            innerQuery.Add(rawQueryParser.Parse("value2"), Matches.Always.GetLuceneOccurance());
            innerQuery.Add(rawQueryParser.Parse("value3"), Matches.Always.GetLuceneOccurance());
            originalQuery.Add(innerQuery, Matches.Always.GetLuceneOccurance());
            string queryString = originalQuery.ToString();
            originalQuery.Add(rawQueryParser.Parse("value2"), Matches.Always.GetLuceneOccurance());

            builder.Setup(x => x.Keywords("_name", new []{"Value", "Value2", "Value3"}));
            Query replacementQuery = builder.Build();
            string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			Query replacementQuery1 = builder.Build();
			string newQueryString1 = replacementQuery1.ToString();

			Assert.AreEqual(queryString, newQueryString1);

			QueryBuilder builder2 = new QueryBuilder();
			builder2.Setup
				(
					x => x.Term("_name", "value1", Matches.Sometimes),
					x => x.Term("_name", "value2", Matches.Sometimes)
				);
			Query replacementQuery2 = builder2.Build();
			string newQueryString2 = replacementQuery2.ToString();

			Assert.AreEqual(queryString, newQueryString2);

			QueryBuilder builder3 = new QueryBuilder();
			builder3.Setup
				(
					x => x.Term("_name", "value1"),
					x => x.Term("_name", "value2")
				);
			Query replacementQuery3 = builder3.Build();
			string newQueryString3 = replacementQuery3.ToString();

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
            Query replacementQuery1 = builder.Build();
            string newQueryString1 = replacementQuery1.ToString();

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
			builder.Or
				(
					x => x.Term("_name", "value1"),
					x => x.Term("_name", "value2")
				);
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
			builder.And
				(
					x => x.Term("_name", "value1"),
					x => x.Term("_name", "value2")
				);
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
					x => x.Phrase(2).AddTerm("_name", "phrase"),
					x => x.WildCard("_name", "*wildcard*", Matches.Sometimes)
				);
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

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
            Query setupQuery = setupBuilder.Build();
            string setupQueryString = setupQuery.ToString();

            QueryBuilder constructorBuilder = new QueryBuilder(x => x.Term("_name", "Value"));
            Query constructorQuery = constructorBuilder.Build();
            string constructorQueryString = constructorQuery.ToString();

            Assert.AreEqual(setupQueryString, constructorQueryString);
        }

        #endregion

        #region [ Raw Tests ]

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
            Query replacementQuery = builder.Build();
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
            Query replacementQuery = builder.Build();
            string newQueryString = replacementQuery.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        #endregion
    }
}
