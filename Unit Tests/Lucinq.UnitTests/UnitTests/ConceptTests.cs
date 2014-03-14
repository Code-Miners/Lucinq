using System;
using System.Diagnostics;
using System.IO;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucinq.Enums;
using Lucinq.Interfaces;
using Lucinq.Querying;
using Lucinq.UnitTests.IntegrationTests;
using NUnit.Framework;

namespace Lucinq.UnitTests.UnitTests
{
    using System.Collections.Generic;
    using System.Configuration;

    [TestFixture]
    public class ConceptTests : BaseTestFixture
    {
        /// <summary>
        /// Test to show the speed of opening / closing ram directory
        /// </summary>
        [Test]
        public void OpeningClosingAll()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Opening FS Dir");
            FSDirectory fileSystemDirectory = FSDirectory.Open(new DirectoryInfo(GeneralConstants.Paths.CarDataIndex));
            WriteTime(stopwatch);
            Console.WriteLine("Opening Ram Dir");
            RAMDirectory ramDirectory = new RAMDirectory(fileSystemDirectory);
            WriteTime(stopwatch);
            ramDirectory.Dispose();
            WriteTime(stopwatch);
            Console.WriteLine("Disposed Ram Dir");
            fileSystemDirectory.Dispose();
            WriteTime(stopwatch);
            Console.WriteLine("Disposed FS Dir");
            stopwatch.Stop();
        }

        /// <summary>
        /// Test to show the speed of opening / closing fs directory
        /// </summary>
        [Test]
        public void OpeningClosingFsOnlyObjects()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Opening FS Dir");
            FSDirectory fileSystemDirectory = FSDirectory.Open(new DirectoryInfo(GeneralConstants.Paths.CarDataIndex));
            WriteTime(stopwatch);
            fileSystemDirectory.Dispose();
            WriteTime(stopwatch);
            Console.WriteLine("Disposed FS Dir");
            stopwatch.Stop();
        }


        [Test]
        public void OpenCloseOpenClose()
        {
            OpeningClosingAll();
            OpeningClosingAll();
            OpeningClosingAll();
            OpeningClosingAll();
            OpeningClosingAll();
            OpeningClosingAll();
            OpeningClosingAll();
            OpeningClosingAll();
            OpeningClosingAll();
        }

        [Test]
        public void OpenCloseFsOnly()
        {
            OpeningClosingFsOnlyObjects();
            OpeningClosingFsOnlyObjects();
            OpeningClosingFsOnlyObjects();
            OpeningClosingFsOnlyObjects();
            OpeningClosingFsOnlyObjects();
            OpeningClosingFsOnlyObjects();
            OpeningClosingFsOnlyObjects();
            OpeningClosingFsOnlyObjects();
        }

        [Test]
        public void LuceneObjectsIntoLucinq()
        {
            LuceneSearch search = new LuceneSearch(new DirectorySearcherProvider(IndexDirectory, false));
            // raw lucene object
            TermQuery query = new TermQuery(new Term(BBCFields.Title, "africa"));
            
            // executed directly by the search
            ILuceneSearchResult result = search.Execute(query);
            Assert.AreEqual(8, result.TotalHits);

            // or by through a querybuilder
            IQueryBuilder queryBuilder = new QueryBuilder();
            queryBuilder.Add(query, Matches.Always);
            ILuceneSearchResult result2 = search.Execute(queryBuilder);
            Assert.AreEqual(8, result2.TotalHits);
        }

        [Test]
        public void BuiltIndexDisposed()
        {
            LuceneSearch luceneSearch = new LuceneSearch(IndexDirectory);
            QueryBuilder qb = new QueryBuilder(x => x.Term(BBCFields.Title, "africa"));
            var result = luceneSearch.Execute(qb);
            result.GetPagedItems(0, 10);
            result.GetTopItems();
        }

        [Test]
        public void ConvertBetweenQueries()
        {
            var conversion = new Conversion();
            FuzzyQuery query1 = conversion.Fuzzy1();
            Console.WriteLine(query1.ToString());

            FuzzyQuery query2 = conversion.Fuzzy2();
            Console.WriteLine(query2.ToString());

            FuzzyQuery query3 = conversion.Fuzzy3();
            Console.WriteLine(query3.ToString());
        }


        public class Conversion : ConversionBase<Query>
        {
            public FuzzyQuery Fuzzy1()
            {
                FuzzyQuery query = new FuzzyQuery(new Term("fred"));
                return this.GetFuzzy1(query, 2.0f);
            }

            public FuzzyQuery Fuzzy2()
            {
                FuzzyQuery query = new FuzzyQuery(new Term("fred"));
                return this.GetFuzzy2(GetWrappedQuery(query), 2.0f);
            }

            public FuzzyQuery Fuzzy3()
            {
                FuzzyQuery query = new FuzzyQuery(new Term("fred"));
                return this.GetFuzzy3(query, 2.0f);
            }

            private IQueryWrapper<T> GetWrappedQuery<T>(T query) where T : Query
            {
                return new QueryWrapper<T>(query);
            }

            protected override IQueryOperations QueryOperations
            {
                get
                {
                    return new QueryOperations();
                }
            }

            protected override void SetBoostValue(Query query, float boost)
            {
                query.Boost = boost;
            }
        }

        public abstract class ConversionBase<TQuery>
        {
            protected abstract IQueryOperations QueryOperations { get; }
            
            protected TActualQuery GetFuzzy1<TActualQuery>(TActualQuery query, float boost)
                where TActualQuery : class, TQuery
            {

                this.SetBoostValue(query, boost);
                return query;
            }

            protected TActualQuery GetFuzzy3<TActualQuery>(TActualQuery query, float boost)
                where TActualQuery : class, TQuery
            {
                this.QueryOperations.SetBoost(query, 2.0f);
                return query;
            }

            protected TActualQuery GetFuzzy2<TActualQuery>(IQueryWrapper<TActualQuery> queryAdapter, float boost)
                where TActualQuery : class, TQuery
            {
                queryAdapter.SetBoostValue(boost);
                return queryAdapter.Query;
            }

            protected abstract void SetBoostValue(TQuery query, float boost);
        }

        public class QueryOperations : IQueryOperations
        {
            public void SetBoost(object query, float boost)
            {
                Query actualQuery = query as Query;
                if (actualQuery == null)
                {
                    return;
                }
                actualQuery.Boost = boost;
            }

            public void SetSlop(object query, int slop)
            {
                PhraseQuery actualQuery = query as PhraseQuery;
                if (actualQuery == null)
                {
                    return;
                }
                actualQuery.Slop = slop;
            }
        }

        public interface IQueryOperations
        {
            void SetBoost(object query, float boost);

            void SetSlop(object query, int slop);
        }

        public class QueryWrapper<TQuery> : IQueryWrapper<TQuery> 
            where TQuery : Query
        {
            public QueryWrapper(TQuery query)
            {
                Query = query;
            }

            public TQuery Query { get; private set; }

            public void SetBoostValue(float boost)
            {
                Query.Boost = boost;
            }
        }

        public interface IQueryWrapper<out TQuery>
        {
            TQuery Query { get; }

            void SetBoostValue(float boost);
        }

        private void WriteTime(Stopwatch stopwatch)
        {
            Console.Write(stopwatch.ElapsedTicks + " - ");
            Console.Write(stopwatch.ElapsedMilliseconds + "\r\n");
        }
    }
}
