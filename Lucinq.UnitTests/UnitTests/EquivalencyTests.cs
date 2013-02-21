using System;
using Lucene.Net.Index;
using Lucene.Net.Search;
using NUnit.Framework;

namespace Lucinq.UnitTests.UnitTests
{
	[TestFixture]
	public class EquivalencyTests
	{
		[Test]
		public void SimpleTermTests()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			TermQuery termQuery = new TermQuery(term);
			originalQuery.Add(termQuery, BooleanClause.Occur.MUST);
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "value"));
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void SimplePhraseTests()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			PhraseQuery phraseQuery = new PhraseQuery();
			phraseQuery.Add(term);
			originalQuery.Add(phraseQuery, BooleanClause.Occur.MUST);
			string queryString = originalQuery.ToString();


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Phrase("_name", "value"));
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void SimpleWildcardTests()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value*");
			WildcardQuery wildcardQuery = new WildcardQuery(term);
			originalQuery.Add(wildcardQuery, BooleanClause.Occur.MUST);
			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.WildCard("_name", "value*"));
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void SimpleOrTests()
		{
			BooleanQuery originalQuery = new BooleanQuery();

			Term term = new Term("_name", "value1");
			TermQuery termQuery1 = new TermQuery(term);
			originalQuery.Add(termQuery1, BooleanClause.Occur.SHOULD);

			Term term2 = new Term("_name", "value2");
			TermQuery termQuery2 = new TermQuery(term2);
			originalQuery.Add(termQuery2, BooleanClause.Occur.SHOULD);

			string queryString = originalQuery.ToString();

			QueryBuilder builder = new QueryBuilder{ChildrenOccur = BooleanClause.Occur.SHOULD};
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
		public void SimpleOrExtensionTests()
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
		public void SimpleAndExtensionTests()
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

		[Test]
		public void CompositeTermPhraseWildcardTests()
		{
			BooleanQuery originalQuery = new BooleanQuery();
			Term term = new Term("_name", "value");
			TermQuery termQuery = new TermQuery(term);
			originalQuery.Add(termQuery, BooleanClause.Occur.MUST);
			PhraseQuery phraseQuery = new PhraseQuery();
			Term phraseTerm = new Term("_name", "phrase");
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
					x => x.Phrase("_name", "phrase"),
					x => x.WildCard("_name", "*wildcard*", BooleanClause.Occur.SHOULD)
				);
			Query replacementQuery = builder.Build();
			string newQueryString = replacementQuery.ToString();

			Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}
	}
}
