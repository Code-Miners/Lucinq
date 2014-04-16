using Lucene.Net.Search;
using Lucinq.Enums;

namespace Lucinq.Interfaces
{
    /// <summary>
    /// The query reference interface
    /// </summary>
    public interface IQueryReference
    {
        /// <summary>
        /// Gets or sets the matches value for the reference
        /// </summary>
        Matches Occur { get; set; }

        /// <summary>
        /// Gets or sets the query
        /// </summary>
        Query Query { get; set; }
    }
}
