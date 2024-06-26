﻿using System.Collections.Generic;
using Lucinq.Core.Enums;
using Lucinq.Core.Querying;

namespace Lucinq.Core.Interfaces
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
		/// Gets the child queries in the builder
		/// </summary>
		Dictionary<string, LucinqQuery> Queries { get; }

		/// <summary>
		/// Gets the child groups in the builder
		/// </summary>
		List<IQueryBuilder> Groups { get; }

		#endregion
	}
}
