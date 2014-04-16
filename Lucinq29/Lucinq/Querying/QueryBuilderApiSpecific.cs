using System;
using Lucene.Net.Search;
using Lucinq.Enums;
using Version = Lucene.Net.Util.Version;

namespace Lucinq.Querying
{
    public partial class QueryBuilder
    {
        /// <summary>
        /// Gets the current lucene version
        /// </summary>
        public static Version CurrentVersion
        {
            get { return Version.LUCENE_29; }
        }

        #region [ Numeric Range Queries ]

        public virtual NumericRangeQuery NumericRange(string fieldName, int minValue, int maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                                    int precisionStep = 1, bool includeMin = true, bool includeMax = true, string key = null)
        {
            NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewIntRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
            SetBoostValue(numericRangeQuery, boost);
            Add(numericRangeQuery, occur, key);
            return numericRangeQuery;
        }

        public virtual NumericRangeQuery NumericRange(string fieldName, float minValue, float maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                                    int precisionStep = 1, bool includeMin = true, bool includeMax = true, string key = null)
        {
            NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewFloatRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
            SetBoostValue(numericRangeQuery, boost);
            Add(numericRangeQuery, occur, key);
            return numericRangeQuery;
        }

        public virtual NumericRangeQuery NumericRange(string fieldName, double minValue, double maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                            int precisionStep = 1, bool includeMin = true, bool includeMax = true, string key = null)
        {
            NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewDoubleRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
            SetBoostValue(numericRangeQuery, boost);
            Add(numericRangeQuery, occur, key);
            return numericRangeQuery;
        }

        public virtual NumericRangeQuery NumericRange(string fieldName, long minValue, long maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                    int precisionStep = 1, bool includeMin = true, bool includeMax = true, string key = null)
        {
            NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewLongRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
            SetBoostValue(numericRangeQuery, boost);
            Add(numericRangeQuery, occur, key);
            return numericRangeQuery;
        }

        public virtual NumericRangeQuery DateRange(string fieldName, DateTime minValue, DateTime maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                  int precisionStep = 1, bool includeMin = true, bool includeMax = true, string key = null)
        {
            NumericRangeQuery numericRangeQuery = NumericRangeQuery.NewLongRange(fieldName, precisionStep, minValue.Ticks, maxValue.Ticks, includeMin, includeMax);
            SetBoostValue(numericRangeQuery, boost);
            Add(numericRangeQuery, occur, key);
            return numericRangeQuery;
        }

        #endregion
    }
}
