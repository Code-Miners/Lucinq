using System;
using Lucinq.Core.Interfaces;

namespace Lucinq.Core.Results
{
    public class DelegateItemSearchResult<TDocument, TReturn> : ItemSearchResult<TDocument, TReturn> where TDocument : class
    {
        private readonly Func<TDocument, TReturn> function;

        public DelegateItemSearchResult(ISearchResult<TDocument> nativeSearchResult, Func<TDocument, TReturn> function)
            : base(nativeSearchResult)
        {
            this.function = function;
        }

        public override TReturn GetItem(TDocument document)
        {
            return function(document);
        }
    }

    public class DelegateSearchResultFactory<TDocument, TReturn> where TDocument : class
    {
        private readonly Func<TDocument, TReturn> function;

        public DelegateSearchResultFactory(Func<TDocument, TReturn> function)
        {
            this.function = function;
        }

        public DelegateItemSearchResult<TDocument, TReturn> GetItemResult(ISearchResult<TDocument> nativeSearchResult)
        {
            return new DelegateItemSearchResult<TDocument, TReturn>(nativeSearchResult, function);
        }

    }
}
