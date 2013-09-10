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
		Equality Occur { get; set; }

		/// <summary>
		/// Gets or sets the default occur value for child queries within the builder
		/// </summary>
        Equality DefaultChildrenOccur { get; set; }

		/// <summary>
		/// Gets the parent query builder
		/// </summary>
		IQueryBuilder Parent { get; }

		/// <summary>
		/// Gets the child queries in the builder
		/// </summary>
		Dictionary<string, QueryReference> Queries { get; }

		/// <summary>
		/// Gets the child groups in the builder
		/// </summary>
		List<IQueryBuilder> Groups { get; }

		Sort CurrentSort { get; set; }

		#endregion
	}
}
