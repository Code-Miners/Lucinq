using Lucene.Net.Search;

namespace Lucinq.Lucene30.Adapters
{
    public class LuceneModel
    {
        public LuceneModel()
        {
            Query = new BooleanQuery();
        }

        public BooleanQuery Query { get; }

        public Sort Sort { get; set; }

        public Filter Filter { get; set; }
    }
}
