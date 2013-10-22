using System;
using Lucene.Net.Documents;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
    public class DelegateItemSearchResult<T> : ItemSearchResult<T>
    {
        private readonly Func<Document, T> function;

        public DelegateItemSearchResult(ILuceneSearchResult<Document> luceneSearchResult, Func<Document, T> function)
            : base(luceneSearchResult)
        {
            this.function = function;
        }

        public override T GetItem(Document document)
        {
            return function(document);
        }
    }
}
