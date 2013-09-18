using System;
using System.Linq;
using System.Linq.Expressions;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
    public class LucinqQueryProvider : IQueryProvider
    {
        #region [ Properties ]

        public ILuceneSearch<LuceneSearchResult> LuceneSearch { get; private set; } 

        public IQueryBuilder QueryBuilder { get; private set; }

        #endregion

        #region [ Methods ]

        public LucinqQueryProvider(ILuceneSearch<LuceneSearchResult> luceneSearch)
        {
            LuceneSearch = luceneSearch;
            QueryBuilder = new QueryBuilder();
        }

        #endregion

        public IQueryable CreateQuery(Expression expression)
        {
            return new LuceneQueryable<IQueryOperatorContainer>(LuceneSearch);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new LuceneQueryable<TElement>(LuceneSearch);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            //return LuceneSearch.Execute(builder);
            throw new NotImplementedException();
        }
    }
}
