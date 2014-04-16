using Lucene.Net.Search;
using Lucinq.Enums;
using Lucinq.Interfaces;

namespace Lucinq.Querying
{
    /// <summary>
    /// The query reference
    /// </summary>
	public class QueryReference : IQueryReference
	{
        /// <summary>
        /// Gets or sets the matches value for the reference
        /// </summary>
		public Matches Occur { get; set; }

        /// <summary>
        /// Gets or sets the query
        /// </summary>
		public Query Query { get; set; }
	}
}
