using System;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucinq.Extensions;
using Lucinq.Querying;
using NUnit.Framework;

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
			originalQuery.Add(termQuery, BooleanClause.Occur.MUST);
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
			originalQuery.Add(termQuery, BooleanClause.Occur.MUST);
			termQuery.SetBoost(10);
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
			originalQuery.Add(termQuery, BooleanClause.Occur.MUST);
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
			originalQuery.Add(termQuery, BooleanClause.Occur.MUST);
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
			originalQuery.Add(termQuery, BooleanClause.Occur.SHOULD);
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value", BooleanClause.Occur.SHOULD));
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
			originalQuery.Add(termQuery, BooleanClause.Occur.SHOULD);
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value", BooleanClause.Occur.SHOULD, caseSensitive: true));
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
			originalQuery.Add(termQuery, BooleanClause.Occur.MUST);
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			TermQuery termQuery2 = new TermQuery(term);
			builder.Add(termQuery2, BooleanClause.Occur.MUST);
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
			phraseQuery.SetSlop(2);
			phraseQuery.Add(term);
			originalQuery.Add(phraseQuery, BooleanClause.Occur.MUST);
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
			phraseQuery.SetSlop(2);
			phraseQuery.Add(term);
			phraseQuery.SetBoost(10);
			originalQuery.Add(phraseQuery, BooleanClause.Occur.MUST);
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
			phraseQuery.SetSlop(2);
			phraseQuery.Add(term);
			originalQuery.Add(phraseQuery, BooleanClause.Occur.MUST);
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
			phraseQuery.SetSlop(2);
			phraseQuery.Add(term);
			originalQuery.Add(phraseQuery, BooleanClause.Occur.MUST);
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
			originalQuery.Add(wildcardQuery, BooleanClause.Occur.MUST);
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
			originalQuery.Add(wildcardQuery, BooleanClause.Occur.MUST);
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
			originalQuery.Add(wildcardQuery, BooleanClause.Occur.MUST);
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
			originalQuery.Add(wildcardQuery, BooleanClause.Occur.SHOULD);
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.WildCard("_name", "Value*", BooleanClause.Occur.SHOULD));
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
			originalQuery.Add(wildcardQuery, BooleanClause.Occur.MUST);
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.WildCard("_name", "Value*", caseSensitive: true));
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
			originalQuery.Add(termRangeQuery, BooleanClause.Occur.MUST);
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
			originalQuery.Add(termRangeQuery, BooleanClause.Occur.MUST);
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.TermRange("field", "Lower", "Upper", caseSensitive:true));
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
			originalQuery.Add(termRangeQuery, BooleanClause.Occur.MUST);
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
			builder.Setup(x => x.TermRange("field", "Lower", "Upper"));
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
			NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewIntRange("field", 1, 0, 10, true, true);
			originalQuery.Add(numericRangeQuery, BooleanClause.Occur.MUST);
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
			NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewDoubleRange("field", 1, 0d, 10d, true, true);
			originalQuery.Add(numericRangeQuery, BooleanClause.Occur.MUST);
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
			NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewLongRange("field", 1, 0L, 10L, true, true);
			originalQuery.Add(numericRangeQuery, BooleanClause.Occur.MUST);
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
			originalQuery.Add(termQuery1, BooleanClause.Occur.SHOULD);

			Term term2 = new Term("_name", "value2");
			TermQuery termQuery2 = new TermQuery(term2);
			originalQuery.Add(termQuery2, BooleanClause.Occur.SHOULD);

			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder{DefaultChildrenOccur = BooleanClause.Occur.SHOULD};
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
					x => x.Term("_name", "value1", BooleanClause.Occur.SHOULD),
					x => x.Term("_name", "value2", BooleanClause.Occur.SHOULD)
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
		public void OrExtension()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			BooleanQuery innerQuery = new BooleanQuery();
			
			Term term = new Term("_name", "value1");
			TermQuery termQuery1 = new TermQuery(term);
			innerQuery.Add(termQuery1, BooleanClause.Occur.SHOULD);

			Term term2 = new Term("_name", "value2");
			TermQuery termQuery2 = new TermQuery(term2);
			innerQuery.Add(termQuery2, BooleanClause.Occur.SHOULD);
			
			originalQuery.Add(innerQuery, BooleanClause.Occur.MUST);
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
			innerQuery.Add(termQuery1, BooleanClause.Occur.MUST);

			Term term2 = new Term("_name", "value2");
			TermQuery termQuery2 = new TermQuery(term2);
			innerQuery.Add(termQuery2, BooleanClause.Occur.MUST);

			originalQuery.Add(innerQuery, BooleanClause.Occur.MUST);
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
			originalQuery.Add(termQuery, BooleanClause.Occur.MUST);
			PhraseQuery phraseQuery = new PhraseQuery();
			Term phraseTerm = new Term("_name", "phrase");
			phraseQuery.SetSlop(2);
			phraseQuery.Add(phraseTerm);
			originalQuery.Add(phraseQuery, BooleanClause.Occur.MUST);

			Term wildcardTerm = new Term("_name", "*wildcard*");
			WildcardQuery wildcardQuery = new WildcardQuery(wildcardTerm);
			originalQuery.Add(wildcardQuery, BooleanClause.Occur.SHOULD);

			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup
				(
					x => x.Term("_name", "value"),
					x => x.Phrase(2).AddTerm("_name", "phrase"),
					x => x.WildCard("_name", "*wildcard*", BooleanClause.Occur.SHOULD)
				);
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		#endregion
	}
}
