using System;
using Lucene.Net.Documents;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
    public class DelegateItemSearchResult<T> : ItemSearchResult<T>
    {
        private readonly Func<Document, T> function;

        public DelegateItemSearchResult(ILuceneSearchResult luceneSearchResult, Func<Document, T> function)
            : base(luceneSearchResult)
        {
            this.function = function;
        }

        public override T GetItem(Document document)
        {
            return function(document);
        }
    }

    public class DelegateSearchResultFactory<T>
    {
        private readonly Func<Document, T> function;

        public DelegateSearchResultFactory(Func<Document, T> function)
        {
            this.function = function;
        }

        public DelegateItemSearchResult<T> GetItemResult(ILuceneSearchResult luceneSearchResult)
        {
            return new DelegateItemSearchResult<T>(luceneSearchResult, function);
        }

    }
}
