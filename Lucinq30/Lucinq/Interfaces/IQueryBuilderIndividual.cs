using System;
using Lucene.Net.Search;
using Lucinq.Enums;

namespace Lucinq.Interfaces
{
    public interface IQueryBuilderIndividual
    {
	    PrefixQuery PrefixedWith(String fieldname, String value, Matches occur = Matches.NotSet, float? boost = null, String key = null);

        /// <summary>
        /// Creates a query using the keyword analyzer
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <param name="caseSensitive"></param>
        /// <returns></returns>
        Query Keyword(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null);

        /// <summary>
        /// Sets up and adds a fuzzy query object allowing the search for an explcit term in the field
        /// </summary>
        /// <param name="fieldName">The field name to search within</param>
        /// <param name="fieldValue">The value to match</param>
        /// <param name="occur">Whether it must, must not or should occur in the field</param>
        /// <param name="boost">A boost multiplier (1 is default / normal).</param>
        /// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
        /// <param name="caseSensitive">Whether the value is explicitly case sensitive (else use the query builders value)</param>
        /// <returns>The generated fuzzy query object</returns>
        FuzzyQuery Fuzzy(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null);

        /// <summary>
        /// Sets up and adds a phrase query object allowing the search for an explcit term in the field
        /// To add terms, use the AddTerm() query extension
        /// </summary>
        /// <param name="occur">Whether it must, must not or should occur in the field</param>
        /// <param name="slop">The allowed distance between the terms</param>
        /// <param name="boost">A boost multiplier (1 is default / normal).</param>
        /// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
        /// <returns>The generated phrase query object</returns>
        PhraseQuery Phrase(int slop, float? boost = null, Matches occur = Matches.NotSet, string key = null);

        /// <summary>
        /// Sets up and adds a wildcard query object allowing the search for an explcit term in the field
        /// </summary>
        /// <param name="fieldName">The field name to search within</param>
        /// <param name="fieldValue">The value to match</param>
        /// <param name="occur">Whether it must, must not or should occur in the field</param>
        /// <param name="boost">A boost multiplier (1 is default / normal).</param>
        /// <param name="key">The dictionary key to allow reference beyond the initial scope</param>
        /// <param name="caseSensitive">Whether the value is explicitly case sensitive (else use the query builders value)</param>
        /// <returns>The generated wildcard query object</returns>
        WildcardQuery WildCard(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null);

        /// <summary>
        /// Querys values to return results within the specified range of terms
        /// </summary>
        /// <param name="fieldName">The field name</param>
        /// <param name="rangeStart">The start of the range to search</param>
        /// <param name="rangeEnd">The end of the range to search</param>
        /// <param name="includeLower">A boolean denoting whether to include the lowest value</param>
        /// <param name="includeUpper"></param>
        /// <param name="occur"></param>
        /// <param name="boost"></param>
        /// <param name="key"></param>
        /// <param name="caseSensitive">Whether the value is explicitly case sensitive (else use the query builders value)</param>
        /// <returns></returns>
        TermRangeQuery TermRange(string fieldName, string rangeStart, string rangeEnd, bool includeLower = true,
                                                bool includeUpper = true,
                                                Matches occur = Matches.NotSet, float? boost = null, string key = null, bool? caseSensitive = null);

        NumericRangeQuery<int> NumericRange(string fieldName, int minValue, int maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                                    int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null);

        NumericRangeQuery<float> NumericRange(string fieldName, float minValue, float maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                                    int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null);

        NumericRangeQuery<double> NumericRange(string fieldName, double minValue, double maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                            int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null);

        NumericRangeQuery<long> NumericRange(string fieldName, long minValue, long maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                    int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null);


        NumericRangeQuery<long> DateRange(String fieldName, DateTime minValue, DateTime maxValue, Matches occur = Matches.NotSet,
                                              float? boost = null, int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, String key = null);

        TermQuery Term(string fieldName, string fieldValue, Matches occur = Matches.NotSet, float? boost = null,
            string key = null, bool? caseSensitive = null);
    }
}
