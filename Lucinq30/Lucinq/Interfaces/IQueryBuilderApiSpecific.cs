using System;
using Lucene.Net.Search;
using Lucinq.Enums;

namespace Lucinq.Interfaces
{
    public interface IQueryBuilderApiSpecific
    {
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
    }
}
