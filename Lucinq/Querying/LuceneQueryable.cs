using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
    public class LuceneQueryable<T> : IQueryable<T>
    {
        #region [ Properties ]

        public Expression Expression { get; private set; }
        public Type ElementType { get; private set; }
        public IQueryProvider Provider { get; private set; }
        public ILuceneSearch<LuceneSearchResult> LuceneSearch { get; private set; } 

        #endregion

        public LuceneQueryable(ILuceneSearch<LuceneSearchResult> luceneSearch)
        {
            Expression = Expression.Constant(this);
            Provider = new LucinqQueryProvider(luceneSearch);
            LuceneSearch = luceneSearch;
        }

        #region [ Methods ]

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return Provider.Execute<LuceneSearchResult>(Expression).GetEnumerator();
        }

        #endregion
    }
}
