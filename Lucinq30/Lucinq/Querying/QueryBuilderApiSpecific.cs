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
            get { return Version.LUCENE_30; }
        }

        #region [ Numeric Range Queries ]

        public virtual NumericRangeQuery<int> NumericRange(string fieldName, int minValue, int maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                            int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null)
        {
            NumericRangeQuery<int> numericRangeQuery = NumericRangeQuery.NewIntRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
            SetBoostValue(numericRangeQuery, boost);
            Add(numericRangeQuery, occur, key);
            return numericRangeQuery;
        }

        public virtual NumericRangeQuery<float> NumericRange(string fieldName, float minValue, float maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                                    int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null)
        {
            NumericRangeQuery<float> numericRangeQuery = NumericRangeQuery.NewFloatRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
            SetBoostValue(numericRangeQuery, boost);
            Add(numericRangeQuery, occur, key);
            return numericRangeQuery;
        }

        public virtual NumericRangeQuery<double> NumericRange(string fieldName, double minValue, double maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                            int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null)
        {
            NumericRangeQuery<double> numericRangeQuery = NumericRangeQuery.NewDoubleRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
            SetBoostValue(numericRangeQuery, boost);
            Add(numericRangeQuery, occur, key);
            return numericRangeQuery;
        }

        public virtual NumericRangeQuery<long> NumericRange(string fieldName, long minValue, long maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                    int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null)
        {
            NumericRangeQuery<long> numericRangeQuery = NumericRangeQuery.NewLongRange(fieldName, precisionStep, minValue, maxValue, includeMin, includeMax);
            SetBoostValue(numericRangeQuery, boost);
            Add(numericRangeQuery, occur, key);
            return numericRangeQuery;
        }

        public virtual NumericRangeQuery<long> DateRange(string fieldName, DateTime minValue, DateTime maxValue, Matches occur = Matches.NotSet, float? boost = null,
                                  int precisionStep = Int32.MaxValue, bool includeMin = true, bool includeMax = true, string key = null)
        {
            NumericRangeQuery<long> numericRangeQuery = NumericRangeQuery.NewLongRange(fieldName, precisionStep, minValue.Ticks, maxValue.Ticks, includeMin, includeMax);
            SetBoostValue(numericRangeQuery, boost);
            Add(numericRangeQuery, occur, key);
            return numericRangeQuery;
        }

        #endregion
    }
}
