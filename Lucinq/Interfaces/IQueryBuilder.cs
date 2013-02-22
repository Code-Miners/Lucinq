using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Lucinq.Querying;

namespace Lucinq.Interfaces
{
	public interface IQueryBuilder
	{
		#region [ Properties ]

		BooleanClause.Occur Occur { get; set; }

		BooleanClause.Occur DefaultChildrenOccur { get; set; }

		IQueryBuilder Parent { get; }

		Dictionary<string, QueryReference> Queries { get; }

		List<IQueryBuilder> Groups { get; }

		#endregion

		#region [ Methods ]

		void Add(Query query, BooleanClause.Occur occur, string key = null);

		Query Build();

		IQueryBuilder End();

		IQueryBuilder Where(Action<IQueryBuilder> inputExpression);

		IQueryBuilder Setup(params Action<IQueryBuilder>[] queries);

		TermQuery Term(string fieldName, string fieldValue, BooleanClause.Occur occur = null,  float? boost = null, string key = null);

		IQueryBuilder Terms(string fieldName, string[] fieldValues, BooleanClause.Occur occur = null, float? boost = null);

		FuzzyQuery Fuzzy(string fieldName, string fieldValue, BooleanClause.Occur occur = null, float? boost = null, string key = null);

		PhraseQuery Phrase(int slop, float? boost = null, BooleanClause.Occur occur = null, string key = null);

		IQueryBuilder Phrase(string fieldName, string[] fieldValues, int slop, BooleanClause.Occur occur = null, float? boost = null);

		WildcardQuery WildCard(string fieldName, string fieldValue, BooleanClause.Occur occur = null, float? boost = null, string key = null);

		IQueryBuilder WildCards(string fieldName, string[] fieldValues, BooleanClause.Occur occur = null,
		                                        float? boost = null);

		IQueryBuilder And(params Action<IQueryBuilder>[] queries);

		IQueryBuilder And(BooleanClause.Occur occur = null, params Action<IQueryBuilder>[] queries);

		IQueryBuilder Or(params Action<IQueryBuilder>[] queries);

		IQueryBuilder Or(BooleanClause.Occur occur = null, params Action<IQueryBuilder>[] queries);

		IQueryBuilder Group(BooleanClause.Occur occur = null, BooleanClause.Occur childrenOccur = null, params Action<IQueryBuilder>[] queries);

		Query Raw(string field, string queryText, BooleanClause.Occur occur = null, string key = null, Analyzer analyzer = null);

		#endregion
	}
}