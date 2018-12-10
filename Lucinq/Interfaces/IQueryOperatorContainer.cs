using Lucinq.Core.Enums;

namespace Lucinq.Core.Interfaces
{
    public interface IQueryOperatorContainer
    {
        bool Keyword(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null,
            string key = null, bool? caseSensitive = null);

        bool Term(string fieldName, string fieldValue);
        bool Term(string fieldName, string fieldValue, float? boost);
        bool Term(string fieldName, string fieldValue, Matches occur);
        bool Term(string fieldName, string fieldValue, Matches occur, float? boost);
        bool Term(string fieldName, string fieldValue, Matches occur, float? boost, string key);
        bool Term(string fieldName, string fieldValue, Matches occur, float? boost, string key, bool? caseSensitive);


    }
}
