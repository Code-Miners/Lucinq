using System.Collections.Generic;
using Lucene.Net.Search;

namespace Lucinq.Interfaces
{
	public interface IQueryBuilder
	{
		BooleanClause.Occur Occur { get; set; }

		IQueryBuilder Parent { get; }

		Dictionary<string, QueryReference> Queries { get; }

		List<IQueryBuilder> Groups { get; } 

		void Add(Query query, BooleanClause.Occur occur, string key = null);

		Query Build();

	}
}
