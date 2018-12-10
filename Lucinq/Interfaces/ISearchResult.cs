using System.Collections.Generic;

namespace Lucinq.Core.Interfaces
{
    public interface ISearchResult<T> : ISearchResult, IEnumerable<T> where T : class
    {
        IList<T> GetRange(int start, int end);

        IList<T> GetTopItems();
    }

    public interface ISearchResult
    {
        /// <summary>
        /// The total matches found
        /// </summary>
        int TotalHits { get; }

        /// <summary>
        /// The elapsed time in ms for the query to execute
        /// </summary>
        long ElapsedTimeMs { get; set; }
    }
}
