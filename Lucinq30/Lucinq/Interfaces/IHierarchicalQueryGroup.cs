using System.Collections.Generic;
using Lucene.Net.Search;
using Lucinq.Enums;
using Lucinq.Querying;

namespace Lucinq.Interfaces
{
	public interface IHierarchicalQueryGroup
	{
		#region [ Properties ]

		/// <summary>
		/// Gets or sets the occurance value for the query builder
		/// </summary>
		Matches Occur { get; set; }

		/// <summary>
		/// Gets or sets the default occur value for child queries within the builder
		/// </summary>
        Matches DefaultChildrenOccur { get; set; }

		/// <summary>
		/// Gets the parent query builder
		/// </summary>
		IQueryBuilder Parent { get; }

		/// <summary>
		/// Gets the child queries in the builder
		/// </summary>
        Dictionary<string, IQueryReference> Queries { get; }

		/// <summary>
		/// Gets the child groups in the builder
		/// </summary>
		List<IQueryBuilder> Groups { get; }

        /// <summary>
        /// Gets the sort fields
        /// </summary>
        List<SortField> SortFields { get; }

        /// <summary>
        /// Gets the current sort 
        /// </summary>
		Sort CurrentSort { get; set; }

		#endregion
	}
}
