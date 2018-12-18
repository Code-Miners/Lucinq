using System;
using System.Collections.Generic;
using Lucinq.AzureSearch.Adapters;
using Lucinq.Core.Enums;
using Lucinq.Core.Querying;
using NUnit.Framework;

namespace Lucinq.AzureSearch.UnitTests.UnitTests
{
	[TestFixture]
	public class EquivalencyTests
	{
		#region [ Terms Tests ]

		[Test]
		public void CaseInsensitiveMandatoryTerm()
		{
			string queryString = "+_name:value";


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value"));
		    LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void BoostedCaseInsensitiveMandatoryTerm()
		{
			string queryString = "+_name:value~10";


			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value", boost:10));
		    LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveMandatoryTerm()
		{
			string queryString = "+_name:Value"; 

			QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value", caseSensitive:true));
		    LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void QueryCaseSensitiveMandatoryTerm()
		{
		    string queryString = "+_name:Value";

            QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
			builder.Setup(x => x.Term("_name", "Value"));
		    LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseInsensitiveNonMandatoryTerm()
		{
		    string queryString = "_name:value";

            QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value", Matches.Sometimes));
		    LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveNonMandatoryTerm()
		{
		    string queryString = "_name:Value";


            QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.Term("_name", "Value", Matches.Sometimes, caseSensitive: true));
		    LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

        [Test]
        public void CaseInsensitiveNotTerm()
        {
            string queryString = "-_name:value";
            
            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.Term("_name", "Value", Matches.Never));
            LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        [Test]
        public void CaseSensitiveNotTerm()
        {
            string queryString = "-_name:Value";
            
            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.Term("_name", "Value", Matches.Never, caseSensitive: true));
            LucinqQueryModel replacementQuery = builder.Build();

            AzureSearchAdapter adapter = new AzureSearchAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

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
		    string queryString = "+_name:\"value\"~2";

            QueryBuilder builder = new QueryBuilder();
		    var terms = new[]
		    {
		        new KeyValuePair<string, string>("_name", "value")
		    };
			builder.Setup(x => x.Phrase(2, terms));
		    LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void BoostedCaseInsensitivePhrase()
		{
		    string queryString = "+_name:\"value\"~2^10";

            QueryBuilder builder = new QueryBuilder();
		    var terms = new[]
		    {
		        new KeyValuePair<string, string>("_name", "Value")
		    };

			builder.Setup(x => x.Phrase(2, terms, 10));
		    LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitivePhrase()
		{
			string queryString = "+_name:\"Value\"~2";

			QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
		    var terms = new[]
		    {
		        new KeyValuePair<string, string>("_name", "Value")
		    };

			builder.Setup(x => x.Phrase(2, terms));
		    LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void QueryCaseSensitivePhrase()
		{
		    string queryString = "+_name:\"Value\"~2";

            QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
		    var terms = new[]
		    {
		        new KeyValuePair<string, string>("_name", "Value")
		    };

			builder.Setup(x => x.Phrase(2, terms));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		#endregion

		#region [ WildCard Tests ]

		[Test]
		public void CaseInsensitiveMandatoryWildCard()
		{
		    string queryString = "+_name:value*";

            QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.WildCard("_name", "Value*"));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveMandatoryWildCard()
		{
		    string queryString = "+_name:Value*";

            QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.WildCard("_name", "Value*", caseSensitive:true));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void QueryCaseSensitiveMandatoryWildCard()
		{
		    string queryString = "+_name:Value*";

            QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
			builder.Setup(x => x.WildCard("_name", "Value*"));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseInsensitiveNonMandatoryWildCard()
		{
		    string queryString = "_name:value*";

            QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.WildCard("_name", "Value*", Matches.Sometimes));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveNonMandatoryWildCard()
		{
		    string queryString = "_name:Value*";

            QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.WildCard("_name", "Value*", Matches.Sometimes, caseSensitive: true));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}


        [Test]
        public void CaseInsensitiveWildCards()
        {
            string queryString = "+_name:value* AND +_name:value2*";

            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.WildCards("_name", new []{"Value*", "Value2*"}));
            LucinqQueryModel replacementQuery = builder.Build();

            AzureSearchAdapter adapter = new AzureSearchAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

		#endregion

		#region [ Term Range Tests ]

		[Test]
		public void CaseInSensitiveTermRange()
		{
		    string queryString = "";

            QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.TermRange("field", "Lower", "Upper"));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveTermRange()
		{
		    string queryString = "";

            QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.TermRange("field", "Lower", "Upper", caseSensitive:true));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

        [Test]
        public void CaseInsensitiveTermRange()
        {
            string queryString = "";

            QueryBuilder builder = new QueryBuilder();
            builder.Setup(x => x.TermRange("field", "Lower", "Upper", caseSensitive: false));
            LucinqQueryModel replacementQuery = builder.Build();

            AzureSearchAdapter adapter = new AzureSearchAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

		[Test]
		public void QueryCaseSensitiveTermRange()
		{
		    string queryString = "";

            QueryBuilder builder = new QueryBuilder{CaseSensitive = true};
			builder.Setup(x => x.TermRange("field", "Lower", "Upper"));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		#endregion

		#region [ Keyword Tests ]

		[Test]
		public void CaseInsensitiveKeyword()
		{
			QueryBuilder builder = new QueryBuilder();

		    string queryString = "+_name:\"value\"";


            builder.Setup(x => x.Keyword("_name", "Value"));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveKeyword()
		{
			QueryBuilder builder = new QueryBuilder();

		    string queryString = "+_name:\"Value\"";


            builder.Setup(x => x.Keyword("_name", "Value", caseSensitive:true));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}


		[Test]
		public void CaseInsensitiveNonMandatoryKeyword()
		{
			QueryBuilder builder = new QueryBuilder();

		    string queryString = "_name:\"value\"";

            builder.Setup(x => x.Keyword("_name", "Value", Matches.Sometimes));
			LucinqQueryModel replacementQuery = builder.Build();
            AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void CaseSensitiveNonMandatoryKeyword()
		{
			QueryBuilder builder = new QueryBuilder();

		    string queryString = "_name:\"Value\"";

            builder.Setup(x => x.Keyword("_name", "Value", Matches.Sometimes, caseSensitive: true));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

        [Test]
        public void CaseInsensitiveKeywords()
        {
            QueryBuilder builder = new QueryBuilder();

            string queryString = "+_name:\"value\" AND +_name:\"value2\" AND +_name:\"value3\"";

            builder.Setup(x => x.Keywords("_name", new []{"Value", "Value2", "Value3"}));
            LucinqQueryModel replacementQuery = builder.Build();

            AzureSearchAdapter adapter = new AzureSearchAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

		#endregion

		#region [ Numeric Range Tests ]

		[Test]
		public void IntegerRange()
		{
		    string queryString = "(field ge 0 and field le 10)";

            QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.NumericRange("field", 0, 10));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).SearchParameters.Filter;

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void DoubleRange()
		{
		    string queryString = "(field ge 0 and field le 10)";

            QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.NumericRange("field", 0d, 10d));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).SearchParameters.Filter;

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		[Test]
		public void LongRange()
		{
		    string queryString = "(field ge 0 and field le 10)";

            QueryBuilder builder = new QueryBuilder();
			builder.Setup(x => x.NumericRange("field", 0L, 10L));
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).SearchParameters.Filter;

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

		#endregion

		#region [ Grouping Tests ]

		[Test]
		public void Or()
		{
		    string queryString = "_name:value1 OR _name:value2";

            QueryBuilder builder = new QueryBuilder{DefaultChildrenOccur = Matches.Sometimes};
			builder.Setup
				(
					x => x.Term("_name", "value1"),
					x => x.Term("_name", "value2")
				);
			LucinqQueryModel replacementQuery1 = builder.Build();
		    AzureSearchAdapter adapter = new AzureSearchAdapter();

		    string newQueryString1 = adapter.Adapt(replacementQuery1).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString1);

			QueryBuilder builder2 = new QueryBuilder();
			builder2.Setup
				(
					x => x.Term("_name", "value1", Matches.Sometimes),
					x => x.Term("_name", "value2", Matches.Sometimes)
				);
			LucinqQueryModel replacementQuery2 = builder2.Build();
		    string newQueryString2 = adapter.Adapt(replacementQuery2).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString2);

			QueryBuilder builder3 = new QueryBuilder();
			builder3.Setup
				(
					x => x.Term("_name", "value1"),
					x => x.Term("_name", "value2")
				);
			LucinqQueryModel replacementQuery3 = builder3.Build();
		    string newQueryString3 = adapter.Adapt(replacementQuery3).QueryBuilder.ToString();

			Assert.AreNotEqual(queryString, newQueryString3);
			
			Console.Write(queryString);
		}

        [Test]
	    public void OptionalOr()
	    {
	        string queryString = "_name:value1 OR _name:value2";

            QueryBuilder builder = new QueryBuilder { DefaultChildrenOccur = Matches.Sometimes };
            builder.Or
                (
                    Matches.Sometimes,
                    x => x.Term("_name", "value1"),
                    x => x.Term("_name", "value2")
                );
            LucinqQueryModel replacementQuery = builder.Build();

	        AzureSearchAdapter adapter = new AzureSearchAdapter();
	        string newQueryString1 = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString1);
	    }

		[Test]
		public void OrExtension()
		{
		    string queryString = "_name:value1 OR _name:value2";

            QueryBuilder builder = new QueryBuilder();
			var builder2 = builder.Or
				(
					x => x.Term("_name", "value1"),
					x => x.Term("_name", "value2")
				);

            Assert.AreEqual(builder2, builder);
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

        [Test]
        public void CreateOrExtension()
        {
            string queryString = "_name:value1 OR _name:value2";

            QueryBuilder builder = new QueryBuilder();
            var group = builder.CreateOrGroup
                (
                    x => x.Term("_name", "value1"),
                    x => x.Term("_name", "value2")
                );
            Assert.AreNotEqual(group, builder);
            LucinqQueryModel replacementQuery = builder.Build();

            AzureSearchAdapter adapter = new AzureSearchAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

		[Test]
		public void AndExtension()
		{
		    string queryString = "+_name:value1 AND +_name:value2";

            QueryBuilder builder = new QueryBuilder();
			var builder2 = builder.And
				(
					x => x.Term("_name", "value1"),
					x => x.Term("_name", "value2")
				);

            Assert.AreEqual(builder2, builder);
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
			Console.Write(queryString);
		}

        [Test]
        public void CreateAndExtension()
        {
            string queryString = "+_name:value1 AND +_name:value2";

            QueryBuilder builder = new QueryBuilder();
            var builder2 = builder.CreateAndGroup
                (
                    x => x.Term("_name", "value1"),
                    x => x.Term("_name", "value2")
                );

            Assert.AreNotEqual(builder2, builder);
            LucinqQueryModel replacementQuery = builder.Build();

            AzureSearchAdapter adapter = new AzureSearchAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

        [Test]
        public void OptionalAndExtension()
        {
            string queryString = "+_name:value1 AND +_name:value2";

            QueryBuilder builder = new QueryBuilder();
            builder.And
                (
                    Matches.Sometimes,
                    x => x.Term("_name", "value1"),
                    x => x.Term("_name", "value2")
                );
            LucinqQueryModel replacementQuery = builder.Build();

            AzureSearchAdapter adapter = new AzureSearchAdapter();
            string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

            Assert.AreEqual(queryString, newQueryString);
            Console.Write(queryString);
        }

		#endregion

		#region [ Composite Tests ]

		[Test]
		public void CompositeTermPhraseWildcardTests()
		{
		    string queryString = "+_name:value OR +_name:\"phrase\"~2 OR _name:*wildcard*";

            QueryBuilder builder = new QueryBuilder();
			builder.Setup
				(
					x => x.Term("_name", "value"),
					x => x.Phrase(2, new []{new KeyValuePair<string, string>("_name", "phrase") }),
					x => x.WildCard("_name", "*wildcard*", Matches.Sometimes)
				);
			LucinqQueryModel replacementQuery = builder.Build();

		    AzureSearchAdapter adapter = new AzureSearchAdapter();
		    string newQueryString = adapter.Adapt(replacementQuery).QueryBuilder.ToString();

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
