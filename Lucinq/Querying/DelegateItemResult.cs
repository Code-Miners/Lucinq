using System;
using Lucene.Net.Documents;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
    public class DelegateItemResult<T> : ItemResult<T>
    {
        private readonly Func<Document, T> function;

        public DelegateItemResult(ILuceneSearchResult<Document> luceneSearchResult, Func<Document, T> function)
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
